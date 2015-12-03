using LScheduler.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LScheduler.Formatter
{
    public class JobTimeFormatter : IJobTimeFormatter
    {
        internal const string EmptyTimeValue = "00";
        internal const string FirstDayOfMonth = "01";

        internal const string TimeStringPattern = "{0}-{1}-{2} {3}:{4}:{5}";

        private string _timeString;

        public string Pattern
        {
            get;
            private set;
        }

        public string RawTime
        {
            get;
            private set;
        }

        private JobDictionary.JobTimeType _type;
        public JobDictionary.JobTimeType Type
        {
            get
            {
                return _type;
            }
        }

        public DateTime FormatTime
        {
            get;
            private set;
        }

        public JobTimeFormatter(string pattern, string rawTime)
        {
            if (String.IsNullOrWhiteSpace(pattern) || String.IsNullOrWhiteSpace(rawTime))
            {
                throw new ArgumentNullException("pattern or rawTime");
            }
            Pattern = pattern;
            RawTime = rawTime;
            _timeString = JobTimeFormatter.TimeStringPattern;
        }

        public void Format()
        {
            var patternItems = Pattern.Split('|');
            var rawTimeItems = RawTime.Split('|');

            if (Enum.TryParse<JobDictionary.JobTimeType>(patternItems[0].ToString(), out _type))
            {
                var index = patternItems.ToList().IndexOf(_type.ToString());
                try
                {
                    switch (_type)
                    {
                        case JobDictionary.JobTimeType.M:
                            //accuracy rating: month
                            _timeString = String.Format(_timeString,
                                DateTime.Now.Year,
                                _dataStringPadding(rawTimeItems[index]),
                                index + 1 >= rawTimeItems.Length ? JobTimeFormatter.FirstDayOfMonth : rawTimeItems[index + 1],
                                index + 2 >= rawTimeItems.Length ? JobTimeFormatter.EmptyTimeValue : rawTimeItems[index + 2],
                                index + 3 >= rawTimeItems.Length ? JobTimeFormatter.EmptyTimeValue : rawTimeItems[index + 3],
                                index + 4 >= rawTimeItems.Length ? JobTimeFormatter.EmptyTimeValue : rawTimeItems[index + 4]);
                            //use 24 hour system - HH:mm:ss
                            FormatTime = DateTime.ParseExact(_timeString, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            if (DateTime.Now.Month <= FormatTime.Month)
                            {
                                FormatTime = FormatTime.AddYears(-1);
                            }

                            break;
                        case JobDictionary.JobTimeType.D:
                            //accuracy rating: day
                            _timeString = String.Format(_timeString,
                                DateTime.Now.Year,
                                _dataIntPadding(DateTime.Now.Month),
                                _dataStringPadding(rawTimeItems[index]),
                                index + 1 >= rawTimeItems.Length ? JobTimeFormatter.EmptyTimeValue : rawTimeItems[index + 1],
                                index + 2 >= rawTimeItems.Length ? JobTimeFormatter.EmptyTimeValue : rawTimeItems[index + 2],
                                index + 3 >= rawTimeItems.Length ? JobTimeFormatter.EmptyTimeValue : rawTimeItems[index + 3]);
                            //use 24 hour system - HH:mm:ss
                            FormatTime = DateTime.ParseExact(_timeString, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            if (DateTime.Now.Day <= FormatTime.Day)
                            {
                                FormatTime = FormatTime.AddMonths(-1);
                            }
                            break;
                        case JobDictionary.JobTimeType.h:
                            //accuracy rating: hour
                            _timeString = String.Format(_timeString,
                               DateTime.Now.Year,
                               _dataIntPadding(DateTime.Now.Month),
                               _dataIntPadding(DateTime.Now.Day),
                               _dataStringPadding(rawTimeItems[index]),
                               index + 1 >= rawTimeItems.Length ? JobTimeFormatter.EmptyTimeValue : rawTimeItems[index + 1],
                               index + 2 >= rawTimeItems.Length ? JobTimeFormatter.EmptyTimeValue : rawTimeItems[index + 2]);
                            //use 24 hour system - HH:mm:ss
                            FormatTime = DateTime.ParseExact(_timeString, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            if (DateTime.Now.Hour <= FormatTime.Hour)
                            {
                                FormatTime = FormatTime.AddDays(-1);
                            }
                            break;
                        case JobDictionary.JobTimeType.m:
                            //accuracy rating: minute
                            _timeString = String.Format(_timeString,
                               DateTime.Now.Year,
                               _dataIntPadding(DateTime.Now.Month),
                               _dataIntPadding(DateTime.Now.Day),
                               _dataIntPadding(DateTime.Now.Hour),
                               _dataStringPadding(rawTimeItems[index]),
                               index + 1 >= rawTimeItems.Length ? JobTimeFormatter.EmptyTimeValue : rawTimeItems[index + 1]);
                            //use 24 hour system - HH:mm:ss
                            FormatTime = DateTime.ParseExact(_timeString, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            if (DateTime.Now.Minute <= FormatTime.Minute)
                            {
                                FormatTime = FormatTime.AddHours(-1);
                            }
                            break;
                        case JobDictionary.JobTimeType.s:
                            //accuracy rating: second
                            _timeString = String.Format(_timeString,
                               DateTime.Now.Year,
                               _dataIntPadding(DateTime.Now.Month),
                               _dataIntPadding(DateTime.Now.Day),
                               _dataIntPadding(DateTime.Now.Hour),
                               _dataIntPadding(DateTime.Now.Minute),
                               _dataStringPadding(rawTimeItems[index]));
                            //use 24 hour system - HH:mm:ss
                            FormatTime = DateTime.ParseExact(_timeString, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            if (DateTime.Now.Second <= FormatTime.Second)
                            {
                                FormatTime = FormatTime.AddMinutes(-1);
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    FormatTime = new DateTime();
                }
            }
        }

        private string _dataIntPadding(int time)
        {
            if (time < 10)
            {
                return time.ToString().PadLeft(2, '0');
            }

            return time.ToString();
        }

        private string _dataStringPadding(string time)
        {
            if (time.Length < 2)
            {
                return time.ToString().PadLeft(2, '0');
            }

            return time;

        }
    }
}
