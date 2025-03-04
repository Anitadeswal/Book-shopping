//using microsoft.extensions.configuration;
//using system;
//using system.collections.generic;
//using system.linq;
//using system.text;
//using twilio;
//using twilio.rest.api.v2010.account;
//using twilio.exceptions;
//using microsoft.extensions.options;
//using twilio.types;

//namespace ecom_project_first_book.utility
//{
//    public interface ismsservice
//    {
//        task sendsmsasync(string phonenumber, string message);
//    }

//    public class twilioservice : ismsservice
//    {
//        private readonly smssettings _smssettings;
//        public twilioservice(ioptions<smssettings> smssettings)
//        {
//            _smssettings = smssettings.value;
//            twilioclient.init(_smssettings.accountsid, _smssettings.authtoken);



//        }

//        public  task sendsmsasync(string phonenumber, string message)
//        {
//            try
//            {
//                var messageoptions = new createmessageoptions(new phonenumber("+91" + phonenumber))
//                {
//                     from = new phonenumber(_smssettings.fromnumber),

//                    body= message
//                };
//                var msg = messageresource.create(messageoptions);
//                return task.fromresult(msg);
                
    
//            }
//            catch (twilioexception ex)
//            {
//                throw new applicationexception("errror occured",ex);
//            }
//        }
//    }
//}
