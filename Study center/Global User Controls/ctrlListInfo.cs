﻿using Study_center.Global_Classes;
using studyCenter_Bl_;
using studyCenter_BL_;
using studyCenter_BusineesLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Study_center.Global_User_Controls
{
    public partial class ctrlListInfo : UserControl
    {

        private DataTable _List;
        private int? _ID;

        public int? ID => _ID;
        public event EventHandler SelectedItem;

        private enum enItemTypes { Subjects = 0, Teachers = 1, MeetingTimes = 2 }
        private enItemTypes _Type;

        public ctrlListInfo()
        {
            InitializeComponent();
        }

        public void SelectItem(int? ID)
        {
            _ID = ID;
            SelectedItem?.Invoke(this, EventArgs.Empty);
        }

        public void FillSubjectsTaughtByTeacher(int? TeacherID)
        {
            _Type = enItemTypes.Subjects;

            clsTeacher teacherInfo = clsTeacher.Find(TeacherID, clsTeacher.EnFindTeacherBy.TeacherID);

            if (teacherInfo != null)
            {
                string prefix = teacherInfo.Gender == clsPerson.EnGender.Male ? "Mr." : "Ms.";
                lblListName.Text = string.Concat("Subjects Taught By Teacher", ' ', prefix, ' ', teacherInfo.FullName);
            }
            _List = clsTeacherSubject.GetSubjectsByTeacherID(TeacherID);
            dgvGradeLevelSubjects.DataSource = _List;
            lblRecordsNum.Text = _List.Rows.Count.ToString();
        }

        public void FillTeachersWhoTeachSubject(int? GradeLevelSubjectID)
        {
            _Type = enItemTypes.Teachers;

            if (!GradeLevelSubjectID.HasValue)
            {
                clsMessages.GeneralErrorMessage("Grade Level Subject ID is required.");
                return;
            }

            clsGradeLevelSubject gradeLevelSubjectInfo = clsGradeLevelSubject.Find(GradeLevelSubjectID);

            if (gradeLevelSubjectInfo == null)
            {
                clsMessages.GeneralErrorMessage($"There is no Grade Level Subject with id {GradeLevelSubjectID} ");
                return;
            }

            lblListName.Text = $"Teachers Who Teach Subject {gradeLevelSubjectInfo.Title}";

            _List = clsTeacherSubject.GetTeachersBySubject(gradeLevelSubjectInfo.SubjectID);
            dgvGradeLevelSubjects.DataSource = _List;
            lblRecordsNum.Text = _List.Rows.Count.ToString();
        }

        public DataTable FillMeetingTimes(int? classID, int? TeacherID)
        {
            _Type = enItemTypes.MeetingTimes;

            if (!classID.HasValue)
            {
                clsMessages.GeneralErrorMessage("class ID is required.");
                return null;
            }
            if (!TeacherID.HasValue)
            {
                clsMessages.GeneralErrorMessage("Teacher ID is required.");
                return null;
            }
            lblListName.Text = $"Available Meeting Times";

            _List = clsGroup.GetAvailableMeetingTimes(classID, TeacherID);
            dgvGradeLevelSubjects.DataSource = _List;
            lblRecordsNum.Text = _List.Rows.Count.ToString();
            return _List;
        }

        private void dgvGradeLevelSubjects_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                switch (_Type)
                {
                    case enItemTypes.Subjects:
                        _ID = (int)dgvGradeLevelSubjects.Rows[e.RowIndex].Cells["GradeLevelSubjectID"].Value;
                        break;
                    case enItemTypes.Teachers:
                        _ID = (int)dgvGradeLevelSubjects.Rows[e.RowIndex].Cells["TeacherID"].Value;
                        break;
                    case enItemTypes.MeetingTimes:
                        _ID = (int)dgvGradeLevelSubjects.Rows[e.RowIndex].Cells["MeetingTimeID"].Value;
                        break;
                }
                SelectedItem?.Invoke(this, EventArgs.Empty);
            }
        }
    }

}
