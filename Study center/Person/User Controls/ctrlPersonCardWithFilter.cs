﻿using Study_center.Global_Classes;
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
using static Study_center.Person.User_Controls.ctrlPersonCardWithFilter;

namespace Study_center.Person.User_Controls
{
    public partial class ctrlPersonCardWithFilter : UserControl
    {
        public enum EnSearchCriteria { PersonID, StudentID, TeacherID }

        private int? _selectedID;

        #region Declare Event
        public event EventHandler<SelectPersonEventArgs> OnPersonSelectedEvent;

        public int? PersonID;
        // Event args class
        public class SelectPersonEventArgs : EventArgs
        {
            public int? PersonID { get; set; }
            public ctrlPersonCardWithFilter.EnSearchCriteria SearchCriteria { get; set; }
        }
        #endregion

        #region Filter Enabled
        private bool _FilterEnabled = true;

        public bool FilterEnabled
        {
            get => _FilterEnabled;

            set
            {
                _FilterEnabled = value;
                gbFilter.Enabled = _FilterEnabled;
            }
        }
        #endregion

        #region Data Back Event
        private void DataBackEvent(int? personID)
        {
            cbFilter.SelectedIndex = 1;
            txtFilterValue.Text = personID.ToString();
            ctrlPersonCard1.LoadPersonData(personID);
            PersonID = personID;
        }

        #endregion
        public ctrlPersonCardWithFilter()
        {
            InitializeComponent();
        }

        public int? GetSelectedID => _selectedID;

        public void LoadPersonInfo(EnSearchCriteria searchCriteria, int? searchValue)
        {
            _selectedID = searchValue;
            // Load common person data based on search criteria
            clsPerson person = null;

            switch (searchCriteria)
            {
                case EnSearchCriteria.PersonID:
                    person = clsPerson.Find(searchValue);
                    break;
                case EnSearchCriteria.StudentID:
                    clsStudent student = clsStudent.Find(searchValue, clsStudent.EnFindStudentBy.StudentID);
                    person = student?.ToPerson();
                    break;
                case EnSearchCriteria.TeacherID:
                    clsTeacher teacher = clsTeacher.Find(searchValue, clsTeacher.EnFindTeacherBy.TeacherID);
                    person = teacher?.ToPerson();
                    break;
            }

            if (person == null)
            {
                MessageBox.Show("Person not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            PersonID = person.PersonID;
            ctrlPersonCard1.LoadPersonData(PersonID);    


            OnPersonSelectedEvent?.Invoke(this, new SelectPersonEventArgs { PersonID = person.PersonID, SearchCriteria = searchCriteria });
        }

        public void SetSearchCriteria(EnSearchCriteria searchCriteria)
        {
            cbFilter.Items.Clear();

            switch (searchCriteria)
            {
                case EnSearchCriteria.PersonID:
                    cbFilter.Items.Add("PersonID");
                    break;
                case EnSearchCriteria.StudentID:
                    cbFilter.Items.Add("PersonID");
                    cbFilter.Items.Add("StudentID");
                    break;
                case EnSearchCriteria.TeacherID:
                    cbFilter.Items.Add("PersonID");
                    cbFilter.Items.Add("TeacherID");
                    break;
            }

            if (cbFilter.Items.Count > 0)
                cbFilter.SelectedIndex = 0;
        }

        private void FindNow()
        {

            if (cbFilter.SelectedItem == null || string.IsNullOrEmpty(txtFilterValue.Text))
            {
                clsMessages.GeneralErrorMessage("Please select a search criteria and enter a search value.");
                return;
            }

            EnSearchCriteria searchCriteria = (EnSearchCriteria)Enum.Parse(typeof(EnSearchCriteria), cbFilter.SelectedItem.ToString());
            int searchValue;

            if (!int.TryParse(txtFilterValue.Text, out searchValue))
            {
                clsMessages.GeneralErrorMessage("Please enter a valid numeric search value.");
                return;
            }

            LoadPersonInfo(searchCriteria, searchValue);

        }

        public void FilterFocus() => txtFilterValue.Focus();

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmAddPerson frmAddPerson = new frmAddPerson();
            frmAddPerson.PersonIDBack += DataBackEvent;
            frmAddPerson.ShowDialog();
        }

        private void btnSearch_Click(object sender, EventArgs e) => FindNow();

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the pressed key is Enter (character code 13)
            if (e.KeyChar == (char)13)
            {

                btnSearch.PerformClick();
            }
            //this will allow only digits if person id is selected
          e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

    }
}