using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;

namespace HSMS
{
    public partial class FormOption : Form
    {
        private AlarmManager.AlarmIgnoreOption m_optCar = AlarmManager.AlarmIgnoreOption.NONE;
        private AlarmManager.AlarmIgnoreOption m_optEquip = AlarmManager.AlarmIgnoreOption.NONE;
        private AlarmManager.AlarmIgnoreOption m_optZone = AlarmManager.AlarmIgnoreOption.NONE;

        public FormOption()
        {
            InitializeComponent();
        }

        private void InitControls(int nIgnoreTime, int nIgnoreDistance, TextBox textBoxDistance, TextBox textBoxDay, TextBox textBoxHour, TextBox textBoxMinute, TextBox textBoxSecond)
        {
            AlarmManager alarmMgr = FormMain.Instance.AlarmManager;

            int nDay = nIgnoreTime / 24 / 3600;
            int nHour = (nIgnoreTime - nDay * 24 * 3600) / 3600;
            int nMin = (nIgnoreTime - nDay * 24 * 3600 - nHour * 3600) / 60;
            int nSec = nIgnoreTime % 60;

            textBoxDistance.Text = nIgnoreDistance.ToString();
            textBoxDay.Text = nDay.ToString();
            textBoxHour.Text = nHour.ToString();
            textBoxMinute.Text = nMin.ToString();
            textBoxSecond.Text = nSec.ToString();
        }

        private void SetOption(AlarmManager.AlarmIgnoreOption option, TextBox textBoxDistance, TextBox textBoxDay, TextBox textBoxHour, TextBox textBoxMinute, TextBox textBoxSecond)
        {
            if (option == AlarmManager.AlarmIgnoreOption.NONE)
            {
                textBoxDistance.Enabled = textBoxDay.Enabled = textBoxHour.Enabled = textBoxMinute.Enabled = textBoxSecond.Enabled = false;
            }
            else if (option == AlarmManager.AlarmIgnoreOption.IGNORE_FOREVER)
            {
                textBoxDistance.Enabled = textBoxDay.Enabled = textBoxHour.Enabled = textBoxMinute.Enabled = textBoxSecond.Enabled = false;
            }
            else if (option == AlarmManager.AlarmIgnoreOption.IGNORE_TIME)
            {
                textBoxDistance.Enabled = false;
                textBoxDay.Enabled = textBoxHour.Enabled = textBoxMinute.Enabled = textBoxSecond.Enabled = true;
            }
            else if (option == AlarmManager.AlarmIgnoreOption.IGNORE_DISTANCE)
            {
                textBoxDistance.Enabled = true;
                textBoxDay.Enabled = textBoxHour.Enabled = textBoxMinute.Enabled = textBoxSecond.Enabled = false;
            }
            else if (option == AlarmManager.AlarmIgnoreOption.IGNORE_TIME_N_DISTANCE_OR)
            {
                textBoxDistance.Enabled = textBoxDay.Enabled = textBoxHour.Enabled = textBoxMinute.Enabled = textBoxSecond.Enabled = true;
            }
            else if (option == AlarmManager.AlarmIgnoreOption.IGNORE_TIME_N_DISTANCE_AND)
            {
                textBoxDistance.Enabled = textBoxDay.Enabled = textBoxHour.Enabled = textBoxMinute.Enabled = textBoxSecond.Enabled = true;
            }
            else
                return;

            if (textBoxDistance == textBoxDistanceCar)
                m_optCar = option;
            else if (textBoxDistance == textBoxDistanceEquip)
                m_optEquip = option;
            else if (textBoxDistance == textBoxDistanceZone)
                m_optZone = option;
        }

        private void FormOption_Shown(object sender, EventArgs e)
        {
            AlarmManager alarmMgr = FormMain.Instance.AlarmManager;

            InitControls(alarmMgr.IgnoreTimeCar, alarmMgr.IgnoreDistanceCar, textBoxDistanceCar, textBoxDayCar, textBoxHourCar, textBoxMinuteCar, textBoxSecondCar);
            InitControls(alarmMgr.IgnoreTimeEquip, alarmMgr.IgnoreDistanceEquip, textBoxDistanceEquip, textBoxDayEquip, textBoxHourEquip, textBoxMinuteEquip, textBoxSecondEquip);
            InitControls(alarmMgr.IgnoreTimeZone, alarmMgr.IgnoreDistanceZone, textBoxDistanceZone, textBoxDayZone, textBoxHourZone, textBoxMinuteZone, textBoxSecondZone);

            InitRadio(alarmMgr.IgnoreOptionCar, radioNoIgnoreCar, radioIgnoreForeverCar, radioIgnoreDistanceCar, radioIgnoreTimeCar, radioIgnoreTimeNDistance_OR_Car, radioIgnoreTimeNDistance_AND_Car);
            InitRadio(alarmMgr.IgnoreOptionEquip, radioNoIgnoreEquip, radioIgnoreForeverEquip, radioIgnoreDistanceEquip, radioIgnoreTimeEquip, radioIgnoreTimeNDistance_OR_Equip, radioIgnoreTimeNDistance_AND_Equip);
            InitRadio(alarmMgr.IgnoreOptionZone, radioNoIgnoreZone, radioIgnoreForeverZone, radioIgnoreDistanceZone, radioIgnoreTimeZone, radioIgnoreTimeNDistance_OR_Zone, radioIgnoreTimeNDistance_AND_Zone);

            SetOption(alarmMgr.IgnoreOptionCar, textBoxDistanceCar, textBoxDayCar, textBoxHourCar, textBoxMinuteCar, textBoxSecondCar);
            SetOption(alarmMgr.IgnoreOptionEquip, textBoxDistanceEquip, textBoxDayEquip, textBoxHourEquip, textBoxMinuteEquip, textBoxSecondEquip);
            SetOption(alarmMgr.IgnoreOptionZone, textBoxDistanceZone, textBoxDayZone, textBoxHourZone, textBoxMinuteZone, textBoxSecondZone);
        }

        private void InitRadio(AlarmManager.AlarmIgnoreOption option, RadioButton radioNoIgnore, RadioButton radioIgnoreForever, RadioButton radioIgnoreDistance, RadioButton radioIgnoreTime, RadioButton radioIgnoreTimeNDistanceOR, RadioButton radioIgnoreTimeNDistanceAND)
        {
            if (option == AlarmManager.AlarmIgnoreOption.NONE)
                radioNoIgnore.Checked = true;
            else if (option == AlarmManager.AlarmIgnoreOption.IGNORE_FOREVER)
                radioIgnoreForever.Checked = true;
            else if (option == AlarmManager.AlarmIgnoreOption.IGNORE_DISTANCE)
                radioIgnoreDistance.Checked = true;
            else if (option == AlarmManager.AlarmIgnoreOption.IGNORE_TIME)
                radioIgnoreTime.Checked = true;
            else if (option == AlarmManager.AlarmIgnoreOption.IGNORE_TIME_N_DISTANCE_OR)
                radioIgnoreTimeNDistanceOR.Checked = true;
            else if (option == AlarmManager.AlarmIgnoreOption.IGNORE_TIME_N_DISTANCE_AND)
                radioIgnoreTimeNDistanceAND.Checked = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int nDistanceCar, nDayCar, nHourCar, nMinCar, nSecCar;
            int nDistanceEquip, nDayEquip, nHourEquip, nMinEquip, nSecEquip;
            int nDistanceZone, nDayZone, nHourZone, nMinZone, nSecZone;

            if (!CheckTextBox(textBoxDayCar, out nDayCar))
                return;
            if (!CheckTextBox(textBoxHourCar, out nHourCar))
                return;
            if (!CheckTextBox(textBoxMinuteCar, out nMinCar))
                return;
            if (!CheckTextBox(textBoxSecondCar, out nSecCar))
                return;
            if (!CheckTextBox(textBoxDistanceCar, out nDistanceCar))
                return;
            if (!CheckTextBox(textBoxDayEquip, out nDayEquip))
                return;
            if (!CheckTextBox(textBoxHourEquip, out nHourEquip))
                return;
            if (!CheckTextBox(textBoxMinuteEquip, out nMinEquip))
                return;
            if (!CheckTextBox(textBoxSecondEquip, out nSecEquip))
                return;
            if (!CheckTextBox(textBoxDistanceEquip, out nDistanceEquip))
                return;
            if (!CheckTextBox(textBoxDayZone, out nDayZone))
                return;
            if (!CheckTextBox(textBoxHourZone, out nHourZone))
                return;
            if (!CheckTextBox(textBoxMinuteZone, out nMinZone))
                return;
            if (!CheckTextBox(textBoxSecondZone, out nSecZone))
                return;
            if (!CheckTextBox(textBoxDistanceZone, out nDistanceZone))
                return;


            EditOptions editOption = new EditOptions();
            //
            editOption.OptCar = m_optCar;
            editOption.OptEquip = m_optEquip;
            editOption.OptZone = m_optZone;
            //
            editOption.DayCar = nDayCar;
            editOption.HourCar = nHourCar;
            editOption.MinCar = nMinCar;
            editOption.SecCar = nSecCar;
            editOption.DistanceCar = nDistanceCar;
            //
            editOption.DayEquip = nDayEquip;
            editOption.HourEquip = nHourEquip;
            editOption.MinEquip = nMinEquip;
            editOption.SecEquip = nSecEquip;
            editOption.DistanceEquip = nDistanceEquip;
            //
            editOption.DayZone = nDayZone;
            editOption.HourZone = nHourZone;
            editOption.MinZone = nMinZone;
            editOption.SecZone = nSecZone;
            editOption.DistanceZone = nDistanceZone;

            //DBConn conn = new DBConn("HSMS");
            editOption.Update(null);

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        

        private bool CheckTextBox(TextBox textBox, out int nData)
        {
            if (!textBox.Visible || textBox.Text.Length == 0)
                nData = 0;
            else
            {
                if (!int.TryParse(textBox.Text, out nData))
                {
                    MessageBox.Show("0 또는 0보다 큰 정수값만 입력 가능합니다.");
                    textBox.Focus();
                    return false;
                }

                if (nData < 0)
                {
                    MessageBox.Show("0 또는 0보다 큰 정수값만 입력 가능합니다.");
                    textBox.Focus();
                    return false;
                }
            }

            return true;
        }

        private void radioIgnore_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioNoIgnore = null, radioIgnoreForever = null, radioIgnoreTime = null;
            RadioButton radioIgnoreDistance = null, radioIgnoreTimeNDistance_OR = null, radioIgnoreTimeNDistance_AND = null;
            TextBox textBoxDistance = null, textBoxDay = null, textBoxHour = null, textBoxMinute = null, textBoxSecond = null;

            if (sender == radioNoIgnoreCar || sender == radioIgnoreForeverCar || sender == radioIgnoreTimeCar ||
                sender == radioIgnoreDistanceCar || sender == radioIgnoreTimeNDistance_OR_Car || sender == radioIgnoreTimeNDistance_AND_Car)
            {
                radioNoIgnore = radioNoIgnoreCar;
                radioIgnoreForever = radioIgnoreForeverCar;
                radioIgnoreTime = radioIgnoreTimeCar;
                radioIgnoreDistance = radioIgnoreDistanceCar;
                radioIgnoreTimeNDistance_OR = radioIgnoreTimeNDistance_OR_Car;
                radioIgnoreTimeNDistance_AND = radioIgnoreTimeNDistance_AND_Car;

                textBoxDistance = textBoxDistanceCar;
                textBoxDay = textBoxDayCar;
                textBoxHour = textBoxHourCar;
                textBoxMinute = textBoxMinuteCar;
                textBoxSecond = textBoxSecondCar;
            }
            else if (sender == radioNoIgnoreEquip || sender == radioIgnoreForeverEquip || sender == radioIgnoreTimeEquip ||
                sender == radioIgnoreDistanceEquip || sender == radioIgnoreTimeNDistance_OR_Equip || sender == radioIgnoreTimeNDistance_AND_Equip)
            {
                radioNoIgnore = radioNoIgnoreEquip;
                radioIgnoreForever = radioIgnoreForeverEquip;
                radioIgnoreTime = radioIgnoreTimeEquip;
                radioIgnoreDistance = radioIgnoreDistanceEquip;
                radioIgnoreTimeNDistance_OR = radioIgnoreTimeNDistance_OR_Equip;
                radioIgnoreTimeNDistance_AND = radioIgnoreTimeNDistance_AND_Equip;

                textBoxDistance = textBoxDistanceEquip;
                textBoxDay = textBoxDayEquip;
                textBoxHour = textBoxHourEquip;
                textBoxMinute = textBoxMinuteEquip;
                textBoxSecond = textBoxSecondEquip;
            }
            else if (sender == radioNoIgnoreZone || sender == radioIgnoreForeverZone || sender == radioIgnoreTimeZone ||
                sender == radioIgnoreDistanceZone || sender == radioIgnoreTimeNDistance_OR_Zone || sender == radioIgnoreTimeNDistance_AND_Zone)
            {
                radioNoIgnore = radioNoIgnoreZone;
                radioIgnoreForever = radioIgnoreForeverZone;
                radioIgnoreTime = radioIgnoreTimeZone;
                radioIgnoreDistance = radioIgnoreDistanceZone;
                radioIgnoreTimeNDistance_OR = radioIgnoreTimeNDistance_OR_Zone;
                radioIgnoreTimeNDistance_AND = radioIgnoreTimeNDistance_AND_Zone;

                textBoxDistance = textBoxDistanceZone;
                textBoxDay = textBoxDayZone;
                textBoxHour = textBoxHourZone;
                textBoxMinute = textBoxMinuteZone;
                textBoxSecond = textBoxSecondZone;
            }
            else
                return;

            if (radioNoIgnore.Checked)
                SetOption(AlarmManager.AlarmIgnoreOption.NONE, textBoxDistance, textBoxDay, textBoxHour, textBoxMinute, textBoxSecond);
            else if (radioIgnoreForever.Checked)
                SetOption(AlarmManager.AlarmIgnoreOption.IGNORE_FOREVER, textBoxDistance, textBoxDay, textBoxHour, textBoxMinute, textBoxSecond);
            else if (radioIgnoreTime.Checked)
                SetOption(AlarmManager.AlarmIgnoreOption.IGNORE_TIME, textBoxDistance, textBoxDay, textBoxHour, textBoxMinute, textBoxSecond);
            else if (radioIgnoreDistance.Checked)
                SetOption(AlarmManager.AlarmIgnoreOption.IGNORE_DISTANCE, textBoxDistance, textBoxDay, textBoxHour, textBoxMinute, textBoxSecond);
            else if (radioIgnoreTimeNDistance_OR.Checked)
                SetOption(AlarmManager.AlarmIgnoreOption.IGNORE_TIME_N_DISTANCE_OR, textBoxDistance, textBoxDay, textBoxHour, textBoxMinute, textBoxSecond);
            else if (radioIgnoreTimeNDistance_AND.Checked)
                SetOption(AlarmManager.AlarmIgnoreOption.IGNORE_TIME_N_DISTANCE_AND, textBoxDistance, textBoxDay, textBoxHour, textBoxMinute, textBoxSecond);
        }

        private void FormOption_Load(object sender, EventArgs e)
        {


        }


    }
}
