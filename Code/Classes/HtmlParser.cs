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

		public IEnumerable<Apartment> GetAndParse()
		{
			using var client = new WebClient();
			var htmlCode = client.DownloadString(_floorPlanUrl);

			var doc = new HtmlDocument();
			doc.LoadHtml(htmlCode);

			var elementsContainsApartments = doc.DocumentNode.Descendants().Where(CheckIfContainApartmentElement).ToList();
			var apartmentElements = GetApartmentElements(elementsContainsApartments);

			return ExtractApartmentsFromElements(apartmentElements);
		}

		private static IEnumerable<Apartment> ExtractApartmentsFromElements(IEnumerable<HtmlNode> apartmentElements)
		{
			return apartmentElements.Select(x =>
			{
				var name = GetApartmentName(x);
				var availability = byte.Parse(GetApartmentAvailability(x)
					.Replace(Const.Html.Literals.Available, string.Empty).Trim());
				var rent = GetApartmentRent(x);
				var price = GetPriceFromSpan(rent);

				return new Apartment { Name = name, Availability = availability, Rent = price };
			});
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

		private static short? GetPriceFromSpan(string priceSpan)
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