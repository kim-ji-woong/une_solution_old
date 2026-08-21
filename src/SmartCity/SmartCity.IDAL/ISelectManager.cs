using SmartCity.Model;
using System;
using System.Collections.Generic;

namespace SmartCity.IDAL
{
    public interface ISelectManager
    {
        AccountUser SelectAccountUser(int id, out string strErrorMessage);
        List<AccountUser> SelectAccountUsers(Dictionary<AccountUser.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        FacilityType SelectFacilityType(int id, out string strErrorMessage);
        List<FacilityType> SelectFacilityTypes(Dictionary<FacilityType.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        AccountLevel SelectAccountLevel(int id, out string strErrorMessage);
        List<AccountLevel> SelectAccountLevels(Dictionary<AccountLevel.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        AccountSession SelectAccountSession(int id, out string strErrorMessage);
        List<AccountSession> SelectAccountSessions(Dictionary<AccountSession.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        Options SelectOptions(int id, out string strErrorMessage);
        List<Options> SelectOptions(Dictionary<Options.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        FireSensor SelectFireSensor(int id, out string strErrorMessage);
        List<FireSensor> SelectFireSensors(Dictionary<FireSensor.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        FloodSensor SelectFloodSensor(int id, out string strErrorMessage);
        List<FloodSensor> SelectFloodSensors(Dictionary<FloodSensor.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        HeatSensor SelectHeatSensor(int id, out string strErrorMessage);
        List<HeatSensor> SelectHeatSensors(Dictionary<HeatSensor.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        CollapseSensor SelectCollapseSensor(int id, out string strErrorMessage);
        List<CollapseSensor> SelectCollapseSensors(Dictionary<CollapseSensor.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        AlertAlarm SelectAlertAlarm(int id, out string strErrorMessage);
        List<AlertAlarm> SelectAlertAlarms(Dictionary<AlertAlarm.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        FacilityManual SelectFacilityManual(int id, out string strErrorMessage);
        List<FacilityManual> SelectFacilityManuals(Dictionary<FacilityManual.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);

        CompanyMember SelectCompanyMember(int id, out string strErrorMessage);
        List<CompanyMember> SelectCompanyMembers(Dictionary<CompanyMember.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        JobLevel SelectJobLevel(int id, out string strErrorMessage);
        List<JobLevel> SelectJobLevels(Dictionary<JobLevel.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        RegularTeam SelectRegularTeam(int id, out string strErrorMessage);
        List<RegularTeam> SelectRegularTeams(Dictionary<RegularTeam.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
    }
}
