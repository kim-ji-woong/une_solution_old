using System.Collections.Generic;
using TeamEditor.Model.Sop.Team;
using SDMS.Model.Sensor;

namespace ExcelWorker.Rollback
{
    public class TeamRollbackData : IRollbackData
    {
        private List<RegularMember> m_insertMembers = null;
        private List<RegularMember> m_deleteMembers = null;
        private List<RegularMember> m_updateMembers = null;

        private List<Regular> m_insertTeams = null;
        private List<Regular> m_deleteTeams = null;
        private List<Regular> m_updateTeams = null;

        private List<TemporaryMember> m_insertTemporaryMembers = null;
        private List<TemporaryMember> m_deleteTemporaryMembers = null;
        private List<TemporaryMember> m_updateTemporaryMembers = null;

        private List<FacilityManager> m_insertFacilityManagers = null;
        private List<FacilityManager> m_deleteFacilityManagers = null;
        private List<FacilityManager> m_updateFacilityManagers = null;

        private List<BuildingFacilityManager> m_insertBuildingFacilityManagers = null;
        private List<BuildingFacilityManager> m_deleteBuildingFacilityManagers = null;
        private List<BuildingFacilityManager> m_updateBuildingFacilityManagers = null;

        private List<EquipZoneFacilityManager> m_insertEquipZoneFacilityManagers = null;
        private List<EquipZoneFacilityManager> m_deleteEquipZoneFacilityManagers = null;
        private List<EquipZoneFacilityManager> m_updateEquipZoneFacilityManagers = null;

        public void SetInsertRegularMembers(List<RegularMember> members)
        {
            m_insertMembers = members;
        }

        public void SetDeleteRegularMembers(List<RegularMember> members)
        {
            m_deleteMembers = members;
        }

        public void SetUpdateRegularMembers(List<RegularMember> members)
        {
            m_updateMembers = members;
        }

        public void SetInsertRegulars(List<Regular> teams)
        {
            m_insertTeams = teams;
        }

        public void SetDeleteRegulars(List<Regular> teams)
        {
            m_deleteTeams = teams;
        }

        public void SetUpdateRegulars(List<Regular> teams)
        {
            m_updateTeams = teams;
        }

        public void SetInsertTemporaryMembers(List<TemporaryMember> members)
        {
            m_insertTemporaryMembers = members;
        }

        public void SetDeleteTemporaryMembers(List<TemporaryMember> members)
        {
            m_deleteTemporaryMembers = members;
        }

        public void SetUpdateTemporaryMembers(List<TemporaryMember> members)
        {
            m_updateTemporaryMembers = members;
        }

        public void SetInsertFacilityManagers(List<FacilityManager> managers)
        {
            m_insertFacilityManagers = managers;
        }

        public void SetDeleteFacilityManagers(List<FacilityManager> managers)
        {
            m_deleteFacilityManagers = managers;
        }

        public void SetUpdateFacilityManagers(List<FacilityManager> managers)
        {
            m_updateFacilityManagers = managers;
        }

        public void SetInsertBuildingFacilityManagers(List<BuildingFacilityManager> managers)
        {
            m_insertBuildingFacilityManagers = managers;
        }

        public void SetDeleteBuildingFacilityManagers(List<BuildingFacilityManager> managers)
        {
            m_deleteBuildingFacilityManagers = managers;
        }

        public void SetUpdateBuildingFacilityManagers(List<BuildingFacilityManager> managers)
        {
            m_updateBuildingFacilityManagers = managers;
        }

        public void SetInsertEquipZoneFacilityManagers(List<EquipZoneFacilityManager> managers)
        {
            m_insertEquipZoneFacilityManagers = managers;
        }

        public void SetDeleteEquipZoneFacilityManagers(List<EquipZoneFacilityManager> managers)
        {
            m_deleteEquipZoneFacilityManagers = managers;
        }

        public void SetUpdateEquipZoneFacilityManagers(List<EquipZoneFacilityManager> managers)
        {
            m_updateEquipZoneFacilityManagers = managers;
        }

        public bool Rollback(SDMS.IDAL.IDataManager sdmsDataManager, TeamEditor.IDAL.IDataManager teamDataManager)
        {
            if (m_insertMembers != null)
            {
                foreach (RegularMember member in m_insertMembers)
                {
                    if (teamDataManager.GetCreateManager().AddRegularMember(member) == false)
                        return false;
                }
            }

            string strErrorMessage;

            if (m_deleteMembers != null)
            {
                foreach (RegularMember member in m_deleteMembers)
                {
                    if (teamDataManager.GetDeleteManager().DeleteRegularMember(member.ID, out strErrorMessage) == false)
                        return false;
                }
            }

            if (m_updateMembers != null)
            {
                foreach (RegularMember member in m_updateMembers)
                {
                    if (teamDataManager.GetUpdateManager().UpdateRegularMember(member, out strErrorMessage) == false)
                        return false;
                }
            }

            if (m_insertTeams != null)
            {
                foreach (Regular team in m_insertTeams)
                {
                    if (teamDataManager.GetCreateManager().AddRegular(team) == false)
                        return false;
                }
            }

            if (m_deleteTeams != null)
            {
                foreach (Regular team in m_deleteTeams)
                {
                    if (teamDataManager.GetDeleteManager().DeleteRegular(team.ID, out strErrorMessage) == false)
                        return false;
                }
            }

            if (m_updateTeams != null)
            {
                foreach (Regular team in m_updateTeams)
                {
                    if (teamDataManager.GetUpdateManager().UpdateRegular(team, out strErrorMessage) == false)
                        return false;
                }
            }

            if (m_insertTemporaryMembers != null)
            {
                foreach (TemporaryMember member in m_insertTemporaryMembers)
                {
                    if (teamDataManager.GetCreateManager().AddTemporaryMember(member) == false)
                        return false;
                }
            }

            if (m_deleteTemporaryMembers != null)
            {
                foreach (TemporaryMember member in m_deleteTemporaryMembers)
                {
                    if (teamDataManager.GetDeleteManager().DeleteTemporaryMember(member.ID, out strErrorMessage) == false)
                        return false;
                }
            }

            if (m_updateTemporaryMembers != null)
            {
                foreach (TemporaryMember member in m_updateTemporaryMembers)
                {
                    if (teamDataManager.GetUpdateManager().UpdateTemporaryMember(member, out strErrorMessage) == false)
                        return false;
                }
            }

            if (m_insertFacilityManagers != null)
            {
                foreach (FacilityManager manager in m_insertFacilityManagers)
                {
                    if (sdmsDataManager.GetCreateManager().CreateFacilityManager(manager.MemberID, manager.MemberType, manager.FacilityType, manager.DetectType, manager.Description, manager.SiteID) == null)
                        return false;
                }
            }

            if (m_deleteFacilityManagers != null)
            {
                foreach (FacilityManager manager in m_deleteFacilityManagers)
                {
                    if (sdmsDataManager.GetDeleteManager().DeleteFacilityManager(manager.ID, out strErrorMessage) == false)
                        return false;
                }
            }

            if (m_updateFacilityManagers != null)
            {
                foreach (FacilityManager manager in m_updateFacilityManagers)
                {
                    if (sdmsDataManager.GetUpdateManager().UpdateFacilityManager(manager, out strErrorMessage) == false)
                        return false;
                }
            }

            if (m_insertBuildingFacilityManagers != null)
            {
                foreach (BuildingFacilityManager manager in m_insertBuildingFacilityManagers)
                {
                    if (sdmsDataManager.GetCreateManager().CreateBuildingFacilityManager(manager.MemberID, manager.MemberType, manager.FacilityType, manager.DetectType, manager.BuildingID, manager.Description, manager.SiteID) == null)
                        return false;
                }
            }

            if (m_deleteBuildingFacilityManagers != null)
            {
                foreach (BuildingFacilityManager manager in m_deleteBuildingFacilityManagers)
                {
                    if (sdmsDataManager.GetDeleteManager().DeleteBuildingFacilityManager(manager.ID, out strErrorMessage) == false)
                        return false;
                }
            }

            if (m_updateBuildingFacilityManagers != null)
            {
                foreach (BuildingFacilityManager manager in m_updateBuildingFacilityManagers)
                {
                    if (sdmsDataManager.GetUpdateManager().UpdateBuildingFacilityManager(manager, out strErrorMessage) == false)
                        return false;
                }
            }

            if (m_insertEquipZoneFacilityManagers != null)
            {
                foreach (EquipZoneFacilityManager manager in m_insertEquipZoneFacilityManagers)
                {
                    if (sdmsDataManager.GetCreateManager().CreateEquipZoneFacilityManager(manager.MemberID, manager.MemberType, manager.FacilityType, manager.DetectType, manager.EquipZoneID, manager.Description, manager.SiteID) == null)
                        return false;
                }
            }

            if (m_deleteEquipZoneFacilityManagers != null)
            {
                foreach (EquipZoneFacilityManager manager in m_deleteEquipZoneFacilityManagers)
                {
                    if (sdmsDataManager.GetDeleteManager().DeleteEquipZoneFacilityManager(manager.ID, out strErrorMessage) == false)
                        return false;
                }
            }

            if (m_updateEquipZoneFacilityManagers != null)
            {
                foreach (EquipZoneFacilityManager manager in m_updateEquipZoneFacilityManagers)
                {
                    if (sdmsDataManager.GetUpdateManager().UpdateEquipZoneFacilityManager(manager, out strErrorMessage) == false)
                        return false;
                }
            }

            return true;
        }
    }
}
