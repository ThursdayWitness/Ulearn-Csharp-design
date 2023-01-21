using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace Delegates.Reports
{

	public delegate object MakeStatistics(IEnumerable<double> data);

	public delegate string MakeFormat(object tempData, object humData, string caption);
	
	public static class ReportMaker
	{
		public static string MakeMarkdown(object tempData, object humData, string caption)
		{
			var result = new StringBuilder();
			result.Append($"## {caption}\n\n");
			result.Append($" * **Temperature**: {tempData}\n\n");
			result.Append($" * **Humidity**: {humData}\n\n");
			return result.ToString();
		}
		
		public static string MakeHtml(object tempData, object humData, string caption)
		{
			var result = new StringBuilder();
			result.Append($"<h1>{caption}</h1>");
			result.Append("<ul>");
			result.Append($"<li><b>Temperature</b>: {tempData}");
			result.Append($"<li><b>Humidity</b>: {humData}");
			result.Append("</ul>");
			return result.ToString();
		}

		public static object CalculateMeanAndStd(IEnumerable<double> _data)
		{
			var data = _data.ToList();
			var mean = data.Average();
			var std = Math.Sqrt(data.Select(z => Math.Pow(z - mean, 2)).Sum() / (data.Count - 1));

			return new MeanAndStd
			{
				Mean = mean,
				Std = std
			};
		}

		public static object CalculateMedian(IEnumerable<double> data)
		{
			var list = data.OrderBy(z => z).ToList();
			if (list.Count % 2 == 0)
				return (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;
			
			return list[list.Count / 2];
		}

		public static string MakeReport(IEnumerable<Measurement> measurements, MakeFormat formatType, MakeStatistics statType, string caption)
		{
			var data = measurements.ToList();
			var tempData = statType(data.Select(z => z.Temperature));
			var humData = statType(data.Select(z => z.Humidity));
			var result = formatType(tempData, humData, caption);
			return result;
		}
	}

	public static class ReportMakerHelper
	{
		public static string MeanAndStdHtmlReport(IEnumerable<Measurement> data)
		{
			var format = new MakeFormat(ReportMaker.MakeHtml);
			var stats = new MakeStatistics(ReportMaker.CalculateMeanAndStd);
			var report = ReportMaker.MakeReport(data, format, stats, "Mean and Std");
			return report;
		}

		public static string MedianMarkdownReport(IEnumerable<Measurement> data)
		{
			var format = new MakeFormat(ReportMaker.MakeMarkdown);
			var stats = new MakeStatistics(ReportMaker.CalculateMedian);
			var report = ReportMaker.MakeReport(data, format, stats, "Median");
			return report;
		}

		public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> measurements)
		{
			var format = new MakeFormat(ReportMaker.MakeMarkdown);
			var stats = new MakeStatistics(ReportMaker.CalculateMeanAndStd);
			var report = ReportMaker.MakeReport(measurements, format, stats, "Mean and Std");
			return report;
		}

		public static string MedianHtmlReport(IEnumerable<Measurement> measurements)
		{
			var format = new MakeFormat(ReportMaker.MakeHtml);
			var stats = new MakeStatistics(ReportMaker.CalculateMedian);
			var report = ReportMaker.MakeReport(measurements, format, stats, "Median");
			return report;
		}
	}
}
