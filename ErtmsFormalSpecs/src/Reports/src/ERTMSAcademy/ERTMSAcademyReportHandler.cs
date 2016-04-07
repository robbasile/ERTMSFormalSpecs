// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using DataDictionary;
using LibGit2Sharp;
using MigraDoc.DocumentObjectModel;

namespace Reports.ERTMSAcademy
{
    public class ERTMSAcademyReportHandler : ReportHandler
    {
        /// <summary>
        ///     The number of days to consider for the report
        /// </summary>
        public int SinceHowManyDays { get; set; }

        /// <summary>
        ///     The user for the report
        /// </summary>
        public string User { get; set; }

        /// <summary>
        ///     The user's git login
        /// </summary>
        public string GitLogin { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary">The dictionary to use to create the report</param>
        public ERTMSAcademyReportHandler(Dictionary dictionary)
            : base(dictionary)
        {
            CreateFileName("ERTMSAcademyReport");
        }

        /// <summary>
        ///     Provides the repository related to the Dictionary directory
        /// </summary>
        /// <returns></returns>
        protected Repository Repository
        {
            get
            {
                Repository retVal = null;

                string directory = Dictionary.BasePath;
                while (retVal == null && !String.IsNullOrEmpty(directory))
                {
                    try
                    {
                        retVal = new Repository(directory);
                    }
                    catch (Exception)
                    {
                        directory = Path.GetDirectoryName(directory);
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Holds statistics about a single file
        /// </summary>
        private class Statistics
        {
            public string FileName { get; private set; }
            public int Additions { get; private set; }
            public int Deletions { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="fileName"></param>
            /// <param name="additions"></param>
            /// <param name="deletions"></param>
            public Statistics(string fileName, int additions, int deletions)
            {
                FileName = fileName;
                Additions = additions;
                Deletions = deletions;
            }
        }

        /// <summary>
        ///     Holds information about a single commit
        /// </summary>
        private class Activity
        {
            public string User { get; private set; }
            public string Email { get; private set; }
            public DateTimeOffset Date { get; private set; }
            public string Comment { get; private set; }
            public List<Statistics> Statistics { get; private set; }

            public int Additions
            {
                get
                {
                    int retVal = 0;
                    foreach (Statistics statistic in Statistics)
                    {
                        retVal += statistic.Additions;
                    }
                    return retVal;
                }
            }

            public int Deletions
            {
                get
                {
                    int retVal = 0;
                    foreach (Statistics statistic in Statistics)
                    {
                        retVal += statistic.Deletions;
                    }
                    return retVal;
                }
            }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="user"></param>
            /// <param name="date"></param>
            /// <param name="comment"></param>
            public Activity(string user, string email, DateTimeOffset date, string comment)
            {
                User = user;
                Email = email;
                Date = date;
                Comment = comment;
                Statistics = new List<Statistics>();
            }

            /// <summary>
            ///     Generates the statistics message
            /// </summary>
            /// <returns></returns>
            public string getStatistics()
            {
                string retVal = "";

                foreach (Statistics statistic in Statistics)
                {
                    retVal += statistic.FileName + " " + statistic.Additions + " addition(s), " + statistic.Deletions +
                              " deletion(s)\n";
                }
                retVal += Statistics.Count + " file(s) changed, " + Additions + " addition(s), " + Deletions +
                          " deletion(s)";

                return retVal;
            }
        }

        /// <summary>
        ///     Creates a report on the model, according to user's choices
        /// </summary>
        /// <returns>The document created, or null</returns>
        public override Document BuildDocument()
        {
            Document retVal = new Document();

            retVal.Info.Title = "ERTMS Academy report";
            retVal.Info.Author = "ERTMS Solutions";
            retVal.Info.Subject = "ERTMS Academy report";

            ERTMSAcademyReport report = new ERTMSAcademyReport(retVal);

            // Make sure the repository is available
            if (Repository == null)
            {
                report.AddSubParagraph("Configuration");
                report.AddParagraph("Invalid git configuration : cannot access git repository");
                report.CloseSubParagraph();
            }
            else
            {
                List<Activity> ModelActivity = new List<Activity>();
                List<Activity> TestActivity = new List<Activity>();
                foreach (Commit commit in Repository.Commits)
                {
                    TimeSpan span = DateTime.Now - commit.Author.When;
                    if (span.Days > SinceHowManyDays)
                    {
                        break;
                    }

                    if (commit.Author.Email == GitLogin)
                    {
                        if (commit.Message.Contains("EA_MODEL") || commit.Message.Contains("EA_TEST"))
                        {
                            Activity activity = new Activity(commit.Author.Name, commit.Author.Email, commit.Author.When,
                                commit.Message);
                            foreach (Commit other in commit.Parents)
                            {
                                TreeChanges changes = Repository.Diff.Compare(other.Tree, commit.Tree);
                                foreach (TreeEntryChanges change in changes.Modified)
                                {
                                    string path = Path.GetFileName(change.Path);
                                    activity.Statistics.Add(new Statistics(path, change.LinesAdded, change.LinesDeleted));
                                }
                            }

                            if (commit.Message.Contains("EA_MODEL"))
                            {
                                ModelActivity.Add(activity);
                            }
                            else
                            {
                                TestActivity.Add(activity);
                            }
                        }
                    }
                }

                // Create the report
                if (ModelActivity.Count > 0)
                {
                    report.AddSubParagraph("Implementation activity by " + User);

                    foreach (Activity activity in ModelActivity)
                    {
                        reportActivity(report, activity);
                    }
                }

                if (TestActivity.Count > 0)
                {
                    report.AddSubParagraph("Testing activity by " + User);

                    foreach (Activity activity in TestActivity)
                    {
                        reportActivity(report, activity);
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Reports a single activity
        /// </summary>
        /// <param name="report"></param>
        /// <param name="activity"></param>
        private void reportActivity(ERTMSAcademyReport report, Activity activity)
        {
            report.AddTable(new string[] {"Added on " + activity.Date}, new int[] {20, 120});
            report.AddRow("Author", activity.User + "(" + activity.Email + ")");
            report.AddRow("Comment", activity.Comment);
            report.AddRow("Statistics", activity.getStatistics());
            report.AddParagraph("");
        }
    }
}