﻿using StudyCenter_DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studyCenter_BusineesLayer
{
    public class clsSubject
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int? SubjectID { get; set; }
        public string SubjectName { get; set; }

        public clsSubject()
        {
            SubjectID = null;
            SubjectName = string.Empty;
            Mode = enMode.AddNew;
        }

        private clsSubject(int? subjectID, string subjectName)
        {
            SubjectID = subjectID;
            SubjectName = subjectName;
            Mode = enMode.Update;
        }

        private bool _Add()
        {
            SubjectID = clsSubjectData.Add(SubjectName);
            return (SubjectID.HasValue);
        }

        private bool _Update()
        {
            return clsSubjectData.Update(SubjectID.Value, SubjectName);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_Add())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _Update();
            }

            return false;
        }

        public static clsSubject Find(int? subjectID)
        {
            string subjectName = string.Empty;
            bool isFound = clsSubjectData.GetInfoByID(subjectID, ref subjectName);
            return (isFound) ? (new clsSubject(subjectID, subjectName)) : null;
        }

        public static bool Delete(int? subjectID)
            => clsSubjectData.Delete(subjectID);

        public static bool Exists(int? subjectID)
            => clsSubjectData.Exists(subjectID);

        public static bool Exists(string subjectName)
            => clsSubjectData.Exists(subjectName);

        public static DataTable All()
            => clsSubjectData.All();

        public static DataTable AllSubjectNames()
            => clsSubjectData.AllSubjectNames();    

        public static string GetSubjectName(int? subjectID)
            => clsSubjectData.GetSubjectName(subjectID);

        public static int? GetSubjectID(string subjectName)
            => clsSubjectData.GetSubjectID(subjectName);

        public static int Count() => clsSubjectData.Count();
    }

}
