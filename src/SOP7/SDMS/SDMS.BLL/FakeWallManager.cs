using System.Collections.Generic;

namespace SDMS.BLL
{
    using Model.Spatial;
    using IDAL;
    using Models.Response;
    using Models.Request;

    public class FakeWallManager
    {
        public static ResponseFakeWalls GetFakeWalls(ProcessManager processManager, IDataManager dataManager, int nZoneID)
        {
            Dictionary<FakeWall.Fields, object> dicConditions = new Dictionary<FakeWall.Fields, object>();
            dicConditions[FakeWall.Fields.ZoneID] = nZoneID;

            string strErrorMessage;
            List<FakeWall> fakeWalls = dataManager.GetSelectManager().SelectFakeWalls(dicConditions, null, out strErrorMessage);

            if (fakeWalls == null)
                return new ResponseFakeWalls(false, strErrorMessage);

            ResponseFakeWalls response = new ResponseFakeWalls(true, "");
            response.FakeWalls.AddRange(fakeWalls);
            return response;
        }

        public static ResponseUpdateFakeWall UpdateFakeWall(ProcessManager processManager, IDataManager dataManager, RequestUpdateFakeWall request, bool saveUserHistory = true)
        {
            ResponseUpdateFakeWall response = null;

            // 삭제할 데이터 구분 - K.D.R
            if (request.FakeWallID < 0 && request.Mode != (int)RequestUpdateFakeWall.UpdateMode.Delete)
            {
                FakeWall fakeWall = dataManager.GetCreateManager().CreateFakeWall(request.ZoneID, request.X, request.Y, request.Z, request.Rotate, request.Scale);

                if (fakeWall == null)
                    response = new ResponseUpdateFakeWall(false, dataManager.GetCreateManager().GetErrorMessage());
                else
                {
                    response = new ResponseUpdateFakeWall(true, "");
                    response.ID = fakeWall.ID;

                    if (saveUserHistory)
                    {
                        Common.BLL.ProcessManager commonProcessManager =
                        new Common.BLL.ProcessManager(processManager.CommonDataManager, processManager.SopDataManager, processManager.TeamDataManager, dataManager);

                        Common.BLL.SaveManager commonSaveManager = commonProcessManager.GetSaveManager();
                        commonSaveManager.SaveUserHistory_AddFakeWall(request.UserID, fakeWall.ZoneID);
                    }
                }
            }
            else if (request.Mode == (int)RequestUpdateFakeWall.UpdateMode.Delete)
            {
                string strErrorMessage;

                if (dataManager.GetDeleteManager().DeleteFakeWall(request.FakeWallID, out strErrorMessage))
                {
                    response = new ResponseUpdateFakeWall(true, "");
                    response.ID = request.FakeWallID;

                    if (saveUserHistory)
                    {
                        Common.BLL.ProcessManager commonProcessManager =
                        new Common.BLL.ProcessManager(processManager.CommonDataManager, processManager.SopDataManager, processManager.TeamDataManager, dataManager);

                        Common.BLL.SaveManager commonSaveManager = commonProcessManager.GetSaveManager();

                        commonSaveManager.SaveUserHistory_DeleteFakeWall(request.UserID, request.ZoneID);
                    }
                }
                else
                    response = new ResponseUpdateFakeWall(false, strErrorMessage);
            }
            else
            {
                FakeWall fakeWall = new FakeWall();

                fakeWall.ID = request.FakeWallID;
                fakeWall.ZoneID = request.ZoneID;
                fakeWall.X = request.X;
                fakeWall.Y = request.Y;
                fakeWall.Z = request.Z;
                fakeWall.Rotate = request.Rotate;
                fakeWall.Scale = request.Scale;

                string strErrorMessage;
                
                if (dataManager.GetUpdateManager().UpdateFakeWall(fakeWall, out strErrorMessage))
                {
                    response = new ResponseUpdateFakeWall(true, "");
                    response.ID = fakeWall.ID;

                    if (saveUserHistory)
                    {
                        Common.BLL.ProcessManager commonProcessManager =
                        new Common.BLL.ProcessManager(processManager.CommonDataManager, processManager.SopDataManager, processManager.TeamDataManager, dataManager);

                        Common.BLL.SaveManager commonSaveManager = commonProcessManager.GetSaveManager();

                        commonSaveManager.SaveUserHistory_ModifyFakeWall(request.UserID, fakeWall.ZoneID, ToModifyType(request.Mode));
                    }
                }
                else
                    response = new ResponseUpdateFakeWall(false, strErrorMessage);
            }

            return response;
        }

        public static ResponseUpdateFakeWalls UpdateFakeWalls(ProcessManager processManager, IDataManager dataManager, RequestUpdateFakeWalls request)
        {
            ResponseUpdateFakeWalls response = new ResponseUpdateFakeWalls(true, "");

            foreach (RequestUpdateFakeWall data in request.UpdateDatas)
            {
                ResponseUpdateFakeWall responseData = UpdateFakeWall(processManager, dataManager, data, false);

                if (responseData.Success == false)
                    return new ResponseUpdateFakeWalls(false, responseData.Message);
                else
                    response.IDs.Add(responseData.ID);
            }

            if (request.UpdateDatas.Count > 0)
            {
                RequestUpdateFakeWall req = request.UpdateDatas[0];

                Common.BLL.ProcessManager commonProcessManager =
                            new Common.BLL.ProcessManager(processManager.CommonDataManager, processManager.SopDataManager, processManager.TeamDataManager, dataManager);

                Common.BLL.SaveManager commonSaveManager = commonProcessManager.GetSaveManager();

                commonSaveManager.SaveUserHistory_ModifyFakeWall(req.UserID, req.ZoneID, ToModifyType(req.Mode));
            }

            return response;
        }

        private static Common.BLL.SaveManager.ModifyType ToModifyType(int mode)
        {
            if (mode == (int)RequestUpdateFakeWall.UpdateMode.Move)
                return Common.BLL.SaveManager.ModifyType.Move;
            else if (mode == (int)RequestUpdateFakeWall.UpdateMode.Rotate)
                return Common.BLL.SaveManager.ModifyType.Rotate;
            else if (mode == (int)RequestUpdateFakeWall.UpdateMode.Resize)
                return Common.BLL.SaveManager.ModifyType.ChangeSize;

            return Common.BLL.SaveManager.ModifyType.None;
        }
    }
}
