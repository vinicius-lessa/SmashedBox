using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

/*
    https://myaccount.google.com/lesssecureapps this is the direct link to the setting you need to change 
*/

public class ConnectionError : MonoBehaviour
{
    public GameObject connectionTestPanel;
    public GameObject connectFirstButton;    
    public TextMeshProUGUI confirmedText;

    // Mail
    private string bodyMessage;
    private string recipientEmail;
    
    private void Start() {
        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set a new selected object
        EventSystem.current.SetSelectedGameObject(connectFirstButton);
    }
    
    public void TryAgain ()
    {     
        connectionTestPanel.SetActive(true);
        transform.gameObject.SetActive(false);
    }

    public void ReportErrorButton () 
    {        
        bodyMessage = "Atenção, problema reportado no jogo #SMASHED BOX, verificar servidor!. <br><br> SMASHED BOX.";
        recipientEmail = "vinicius.lessa33@outlook.com";

        SendEmail(recipientEmail, bodyMessage);
    }

    public void SendEmail(string recipientEmail, string bodyMessage)
    {
        MailMessage mail = new MailMessage();
        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
        SmtpServer.Timeout = 10000;
        SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
        SmtpServer.UseDefaultCredentials = false;
        SmtpServer.Port = 587;

        mail.From = new MailAddress("vinicius.lessa33@gmail.com");
        mail.To.Add(new MailAddress(recipientEmail));
        
        mail.Subject = "SMASHED BOX - #ERRO 01 (Server did not respond)";
        mail.Body = bodyMessage;
        

        SmtpServer.Credentials = new System.Net.NetworkCredential("vinicius.lessa33@gmail.com", "14054898") as ICredentialsByHost; SmtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        };

        try
        {
            SmtpServer.Send(mail);
            confirmedText.gameObject.SetActive(true);
        }
        catch (SmtpException ex)
        {
            confirmedText.gameObject.SetActive(false);
            Debug.Log("Exception caught in CreateTestMessage2(): {0}" + ex.ToString());
        }

        // mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
        // SmtpServer.Send(mail);
    }

    // public void SendText(string phoneNumber)
    // {
    //     MailMessage mail = new MailMessage();
    //     SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
    //     SmtpServer.Timeout = 10000;
    //     SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
    //     SmtpServer.UseDefaultCredentials = false;

    //     mail.From = new MailAddress("myEmail@gmail.com");

    //     mail.To.Add(new MailAddress(phoneNumber + "@txt.att.net"));//See carrier destinations below
    //                                                                //message.To.Add(new MailAddress("5551234568@txt.att.net"));
    //     mail.To.Add(new MailAddress(phoneNumber + "@vtext.com"));
    //     mail.To.Add(new MailAddress(phoneNumber + "@messaging.sprintpcs.com"));
    //     mail.To.Add(new MailAddress(phoneNumber + "@tmomail.net"));
    //     mail.To.Add(new MailAddress(phoneNumber + "@vmobl.com"));
    //     mail.To.Add(new MailAddress(phoneNumber + "@messaging.nextel.com"));
    //     mail.To.Add(new MailAddress(phoneNumber + "@myboostmobile.com"));
    //     mail.To.Add(new MailAddress(phoneNumber + "@message.alltel.com"));
    //     mail.To.Add(new MailAddress(phoneNumber + "@mms.ee.co.uk"));

    //     mail.Subject = "Subject";
    //     mail.Body = "";

    //     SmtpServer.Port = 587;

    //     SmtpServer.Credentials = new System.Net.NetworkCredential("myEmail@gmail.com", "MyPasswordGoesHere") as ICredentialsByHost; SmtpServer.EnableSsl = true;
    //     ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    //     {
    //         return true;
    //     };

    //     mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
    //     SmtpServer.Send(mail);
    // }

}