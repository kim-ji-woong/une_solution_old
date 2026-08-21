using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace AwsSms
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string strSender = textBoxSender.Text.Trim();

                if (strSender.Length == 0)
                {
                    textBoxSender.Focus();
                    MessageBox.Show("발신자 번호를 입력하세요.");
                    return;
                }

                string strReceiver = textBoxReceivers.Text.Trim();

                if (strReceiver.Length == 0)
                {
                    textBoxReceivers.Focus();
                    MessageBox.Show("수신자 번호를 입력하세요.");
                    return;
                }

                string strMessage = textBoxMessage.Text.Trim();
                
                if (strMessage.Length == 0)
                {
                    textBoxMessage.Focus();
                    MessageBox.Show("전송할 메시지를 입력하세요.");
                    return;
                }

                Amazon.RegionEndpoint region = null;
                AWSCredentials credential = GetCredentials(ref region);

                AmazonSimpleNotificationServiceClient snsClient = new AmazonSimpleNotificationServiceClient(credential, region);
                PublishRequest pubRequest = new PublishRequest();
                pubRequest.Message = strMessage;
                pubRequest.PhoneNumber = GetE164PhoneNumber(strReceiver);
                pubRequest.MessageAttributes.Add("AWS.SNS.SMS.SenderID", new MessageAttributeValue
                { StringValue = strSender, DataType = "String" });
                pubRequest.MessageAttributes.Add("AWS.SNS.SMS.SMSType", new MessageAttributeValue
                { StringValue = "Transactional", DataType = "String" });

                PublishResponse pubResponse = snsClient.Publish(pubRequest);
                System.Diagnostics.Trace.WriteLine("Result : " + pubResponse.HttpStatusCode.ToString() + ", " + pubResponse.MessageId);

                /*Amazon.RegionEndpoint region = null;
                AWSCredentials credential = GetCredentials(ref region);

                AmazonSimpleNotificationServiceClient snsClient = new AmazonSimpleNotificationServiceClient(credential, region);
                PublishRequest pubRequest = new PublishRequest();
                pubRequest.Message = GetMessage();
                pubRequest.PhoneNumber = "+821024411820";
                pubRequest.MessageAttributes.Add("AWS.SNS.SMS.SenderID", new MessageAttributeValue
                { StringValue = "027144133", DataType = "String" });
                pubRequest.MessageAttributes.Add("AWS.SNS.SMS.SMSType", new MessageAttributeValue
                { StringValue = "Transactional", DataType = "String" });

                PublishResponse pubResponse = snsClient.Publish(pubRequest);
                System.Diagnostics.Trace.WriteLine("Result : " + pubResponse.ToString() + ", " + pubResponse.MessageId);*/
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        private string GetE164PhoneNumber(string strPhoneNumber)
        {
            if (strPhoneNumber.StartsWith("0"))
                return "+82" + strPhoneNumber.Substring(1);

            return "+82" + strPhoneNumber;
        }

        private string GetMessage()
        {
            string str = "";

            str += "바람을 타고 날아오르는 ";
            str += "새들은 걱정없이 ";
            str += "아름다운 태양속으로 ";
            str += "음표가 되어 나네 ";
            str += "향기나는 연필로 쓴 ";
            str += "일기처럼 ";
            str += "숨겨두었던 마음 ";
            str += "기댈수 있는 ";
            str += "어깨가 있어 ";
            str += "비가 와도 젖지 않아 ";
            str += "어제의 일들은 잊어 ";
            str += "누구나 조금씩은 틀려 ";
            str += "완벽한 사람은 없어 ";
            str += "실수투성이고 ";
            str += "외로운 나를 봐 ";
            str += "난 다시 ";
            str += "태어난 것만 같아 ";
            str += "그대를 만나고부터 ";
            str += "그대 나의 ";
            str += "초라한 마음을 ";
            str += "받아준 순간부터 ";
            str += "랄랄랄랄랄 ";
            str += "\r\n";
            str += "하루 하루 ";
            str += "조금씩 나아질거야 ";
            str += "그대가 지켜보니 ";
            str += "힘을 내야지 ";
            str += "행복해져야지 ";
            str += "뒷뜰에 핀 꽃들처럼 ";
            str += "점심을 함께 먹어야지 ";
            str += "새로 연 그 가게에서 ";
            str += "새 샴푸를 사러가야지 ";
            str += "아침 하늘빛의 ";
            str += "민트 향기면 어떨까 ";
            str += "난 다시 ";
            str += "꿈을 꾸게 되었어 ";
            str += "그대를 만나고부터 ";
            str += "그대 나의 초라한 ";
            str += "마음을 받아준 순간부터 ";
            str += "\r\n";
            str += "월요일도 화요일도 ";
            str += "봄에도 겨울에도 ";
            str += "해가 질 무렵에도 ";
            str += "비둘기를 안은 ";
            str += "아이같이 ";
            str += "행복해줘 나를 위해서 ";
            str += "난 다시 ";
            str += "태어난 것만 같아 ";
            str += "그대를 만나고부터 ";
            str += "그대 나의 초라한 ";
            str += "마음을 ";
            str += "받아준 순간부터 ";
            str += "난 다시 ";
            str += "꿈을 꾸게 되었어 ";
            str += "그대를 만나고부터 ";
            str += "그대 나의 초라한 ";
            str += "마음을 받아준 순간부터 ";
            str += "랄랄랄랄랄 ";
            str += "랄랄랄랄랄랄랄 ";
            str += "랄랄랄랄랄 ";
            str += "랄랄랄랄랄랄랄 우~ ";
            str += "랄랄랄랄랄 ";
            str += "랄랄랄랄랄랄랄랄랄 ";
            str += "우~ ";

            return str;
        }

        private void SetAttrib()
        {
            PublishRequest pubRequest = new PublishRequest();
            // add optional MessageAttributes...
            pubRequest.MessageAttributes["AWS.SNS.SMS.SenderID"] =
                new MessageAttributeValue { StringValue = "sms_une", DataType = "String" };
            pubRequest.MessageAttributes["AWS.SNS.SMS.MaxPrice"] =
                new MessageAttributeValue { StringValue = "1.00", DataType = "Number" };
            pubRequest.MessageAttributes["AWS.SNS.SMS.SMSType"] =
                new MessageAttributeValue { StringValue = "Promotional", DataType = "String" };

            // "Promotional", "Transactional"
        }

        private AWSCredentials GetCredentials(ref Amazon.RegionEndpoint region)
        {
            // 한국 : APNortheast2
            region = Amazon.RegionEndpoint.USEast1;

            var options = new CredentialProfileOptions
            {
                AccessKey = "AKIA5CI44Y6KCDR5RO73",
                SecretKey = "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"
            };
            var profile = new Amazon.Runtime.CredentialManagement.CredentialProfile("basic_profile", options);

            profile.Region = region;
            var netSDKFile = new NetSDKCredentialsFile();
            netSDKFile.RegisterProfile(profile);

            var chain = new CredentialProfileStoreChain();
            AWSCredentials awsCredentials;
            if (chain.TryGetAWSCredentials("basic_profile", out awsCredentials))
            {
                return awsCredentials;
            }

            return null;
        }
    }
}
