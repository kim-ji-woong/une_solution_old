namespace SVMSEventReciver
{
    partial class SVMSReciverInstaller
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.reciverInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.reciverServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // reciverInstaller
            // 
            this.reciverInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.reciverInstaller.Password = null;
            this.reciverInstaller.Username = null;
            // 
            // reciverServiceInstaller
            // 
            this.reciverServiceInstaller.Description = "SVMS 이벤트 수신자";
            this.reciverServiceInstaller.DisplayName = "SVMSReciver";
            this.reciverServiceInstaller.ServiceName = "SVMSReciver";
            this.reciverServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // SVMSReciverInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.reciverInstaller,
            this.reciverServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller reciverInstaller;
        private System.ServiceProcess.ServiceInstaller reciverServiceInstaller;
    }
}