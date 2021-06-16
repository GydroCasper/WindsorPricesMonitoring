using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using WindsorPricesMonitoring.Code.Constants;
using WindsorPricesMonitoring.Code.Dto;
using WindsorPricesMonitoring.Interfaces;

namespace WindsorPricesMonitoring.Code.Classes
{
	public class HtmlParser: IHtmlParser
	{
		private readonly string _floorPlanUrl;

		public HtmlParser(IConfiguration configuration)
		{
			_floorPlanUrl = configuration["FloorPlansUrl"];
		}

		public (IEnumerable<Apartment>, IEnumerable<Unit>) GetAndParse()
		{
			var html = Download(_floorPlanUrl);

			var elementsContainsApartments = html.Descendants().Where(CheckIfContainApartmentElement).ToList();
			var apartmentElements = GetApartmentElements(elementsContainsApartments);

			return ExtractApartmentsAndUnitsFromElements(apartmentElements);
		}

		private static HtmlNode Download(string url)
		{
			using var client = new WebClient();
			var htmlCode = client.DownloadString(url);

			var doc = new HtmlDocument();
			doc.LoadHtml(htmlCode);
			return doc.DocumentNode;
		}

		private (IEnumerable<Apartment>, IEnumerable<Unit>) ExtractApartmentsAndUnitsFromElements(IEnumerable<HtmlNode> apartmentElements)
		{
			var apartments = new List<Apartment>();
			var units = new List<Unit>();

			foreach (var element in apartmentElements)
			{
				var name = GetApartmentName(element);
				var availability = byte.Parse(GetApartmentAvailability(element)
					.Replace(Const.Html.Literals.Available, string.Empty).Trim());
				var rent = GetApartmentRent(element);
				var price = ParsePrice(rent);

				apartments.Add(new Apartment { Name = name, Availability = availability, Rent = price });

				if (availability > 0)
				{
					var unitsLink = GetUnitsLink(element);

					var apartmentUnits = GetAndParseUnits(name, unitsLink).ToList();

					if (apartmentUnits.Any()) units.AddRange(apartmentUnits);
				}
			}

			return (apartments, units);
		}

		private static string GetUnitsLink(HtmlNode element)
		{
			return element.Descendants()
				.Single(x =>
					x.Name == Const.Html.Elements.Anchor &&
					x.Attributes.Any(y => y.Name == Const.Html.Attributes.Name && y.Value == Const.Html.Anchors.UnitsPageButton))
				.Attributes.Single(x => x.Name == Const.Html.Attributes.Href).Value;
		}

		private IEnumerable<Unit> GetAndParseUnits(string unitType, string unitsLink)
		{
			var uri = new Uri(_floorPlanUrl);

			var html = Download($"{uri.Scheme}://{uri.Host}{unitsLink}");

			var unitRows = html.Descendants().Where(x =>
					x.Name == Const.Html.Elements.TableRow &&
					HasSeleniumAttributeWithValue(x, Const.Html.Anchors.UnitRow))
				.ToList();

			foreach (var unit in unitRows)
			{
				var fullNumber = GetChildNodesOfSeleniumElement(unit, Const.Html.Anchors.Apartment)
					.Single(IsNotEmptyText).InnerText.Trim().Trim('#');

				var rent = GetChildNodesOfSeleniumElement(unit, Const.Html.Anchors.Rent)
					.Single(x => IsNotEmptyText(x) && !x.InnerText.Trim().StartsWith('-')).InnerText.Trim();

				var price = ParsePrice(rent);

				var dateAvailableValue = GetChildNodesOfSeleniumElement(unit, Const.Html.Anchors.DateAvailable)
					.Single(IsNotEmptyText).InnerText.Trim();

				yield return new Unit
				{
					FullNumber = fullNumber, 
					MinimumPrice = price, 
					UnitType = unitType,
					DateAvailable = DateTime.TryParse(dateAvailableValue, out var dateAvailable) ? dateAvailable : null
				};
			}
		}

		private static bool IsNotEmptyText(HtmlNode node)
		{
			return node.NodeType == HtmlNodeType.Text && !string.IsNullOrEmpty(node.InnerText.Trim());
		}

		private static HtmlNodeCollection GetChildNodesOfSeleniumElement(HtmlNode node, string seleniumAttributeValueStartsWith)
		{
			return node.Descendants().Single(x => HasSeleniumAttributeWithValue(x, seleniumAttributeValueStartsWith))
				.ChildNodes;
		}

		private static bool HasSeleniumAttributeWithValue(HtmlNode node, string value)
		{
			return node.Attributes.Any(y => y.Name == Const.Html.Attributes.SeleniumAttribute && y.Value.StartsWith(value));
		}

		private static string GetApartmentName(HtmlNode apartment)
		{
			return GetValueOfElement(apartment, Const.Html.Anchors.Name);
		}

		private static string GetApartmentAvailability(HtmlNode apartment)
		{
			return GetValueOfElement(apartment, Const.Html.Anchors.Availability);
		}

		private static string GetApartmentRent(HtmlNode apartment)
		{
			return GetValueOfElement(apartment, Const.Html.Anchors.Rent);
		}

		private static string GetValueOfElement(HtmlNode apartment, string endsOn)
		{
			return apartment.Descendants().Single(x => CheckIfElementOfPartFloorPlan(endsOn, x)).InnerText.Trim();
		}

		private static bool CheckIfElementOfPartFloorPlan(string endsOn, HtmlNode x)
		{
			return x.Attributes.Any(y =>
				y.Value.StartsWith(Const.Html.Anchors.FloorPlan) &&
				y.Value.EndsWith(endsOn));
		}

		private static bool HasElementsStartsAndEndsOn(HtmlNode node, string startsOn, string endsOn)
		{
			return node.Descendants().Any(x =>
				x.Attributes.Any(y =>
					y.Value.StartsWith(startsOn) && y.Value.EndsWith(endsOn)));
		}

		private static IEnumerable<HtmlNode> GetApartmentElements(ICollection<HtmlNode> priceSpans)
		{
			return priceSpans.Where(x => !x.ChildNodes.Any(priceSpans.Contains));
		}

		private static short? ParsePrice(string priceSpan)
		{
			var dollarSignIndex = priceSpan.IndexOf("$", StringComparison.InvariantCulture);
			if (dollarSignIndex >= 0) priceSpan = priceSpan[(dollarSignIndex + 1)..];

			priceSpan = priceSpan.Replace(",", string.Empty);

			for (var i = 1; i < priceSpan.Length; i++)
			{
				if (int.TryParse(priceSpan[..i], out _)) continue;

				return i != 1 ? short.Parse(priceSpan[..(i - 1)]) : null;
			}

			return short.Parse(priceSpan);
		}

		private static bool CheckIfContainApartmentElement(HtmlNode node)
		{
			return node.NodeType == HtmlNodeType.Element &&
			       HasElementsStartsOnFloorPlanAndEndsOn(node, Const.Html.Anchors.Name) &&
			       HasElementsStartsOnFloorPlanAndEndsOn(node, Const.Html.Anchors.Availability) &&
			       HasElementsStartsOnFloorPlanAndEndsOn(node, Const.Html.Anchors.Rent);
		}

		private static bool HasElementsStartsOnFloorPlanAndEndsOn(HtmlNode node, string endsOn)
		{
			return HasElementsStartsAndEndsOn(node, Const.Html.Anchors.FloorPlan, endsOn);
		}
	}
}