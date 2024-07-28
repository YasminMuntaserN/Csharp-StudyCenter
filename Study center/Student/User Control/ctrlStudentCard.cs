﻿using Study_center.Global_Classes;
using Study_center.Person.User_Controls;
using studyCenter_BusineesLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Study_center.Student.User_Control
{
    public partial class ctrlStudentCard : UserControl
    {
        public event EventHandler StudentSelected;

        private clsStudent _Student;

        public clsStudent StudentInfo => _Student;

        public ctrlStudentCard()
        {
            InitializeComponent();
            ctrlPersonCardWithFilter1.SetSearchCriteria(ctrlPersonCardWithFilter.EnSearchCriteria.StudentID);

        }

        private void FillStudentInfoInFelids()
        {
            ctrlPersonCardWithFilter1.setFilterEnabledAndLoadData(_Student.PersonID);
            lblStudentID.Text = _Student.StudentID.ToString();
            lblGradeLevel.Text = clsGradeLevel.GetGradeLevelName(_Student.GradeLevelID);
            lblEmergencyContactPhone.Text = _Student.EmergencyContactPhone;
            lblEnrollmentDate.Text = clsFormat.DateToShort(_Student.EnrollmentDate);
            lblCreatedBy.Text = _Student.CreatedByUserID.ToString();
        }

        protected virtual void OnStudentSelected(EventArgs e)
        {
            StudentSelected?.Invoke(this, e);
        }

        public void LoadStudentInfo(int? studentID)
        {
            _Student = clsStudent.Find(studentID, clsStudent.EnFindStudentBy.StudentID);

            if (_Student == null)
            {
                MessageBox.Show("Student not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            FillStudentInfoInFelids();
            OnStudentSelected(EventArgs.Empty);
        }

        public void LoadStudentInfoByPerson(int? PersonID)
        {
            _Student = clsStudent.Find(PersonID, clsStudent.EnFindStudentBy.PersonID);

            if (_Student == null)
            {
                MessageBox.Show("Student not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FillStudentInfoInFelids();
            OnStudentSelected(EventArgs.Empty);
        }

        private void ctrlPersonCardWithFilter1_OnPersonSelectedEvent(object sender, Study_center.Person.User_Controls.ctrlPersonCardWithFilter.SelectPersonEventArgs e)
        {
            if (e.SearchCriteria == ctrlPersonCardWithFilter.EnSearchCriteria.PersonID)
            {
                LoadStudentInfoByPerson(ctrlPersonCardWithFilter1.GetSelectedID);
            }
            else if (e.SearchCriteria == ctrlPersonCardWithFilter.EnSearchCriteria.StudentID)
            {
                LoadStudentInfo(ctrlPersonCardWithFilter1.GetSelectedID);
            }
        }

        private void llEditStudentInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmAddStudent frmAddStudent = new frmAddStudent(_Student.StudentID);
            frmAddStudent.ShowDialog();
            LoadStudentInfo(_Student.StudentID);
        }

    }
}
