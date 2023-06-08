using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CSCore;
using CSCore.Codecs;
using pjsua2xamarin;
namespace Sip_EX
{
    public class SoftPhone 
    {
        private readonly object _lock = new object();
        public static Endpoint ep = new Endpoint();
        MyAccount acc = new MyAccount();
        public string RegisterStatus = "not registered";
        public static MyCall activeCall;
        CallOpParam callOpParam;
        public void init()
        {
            callOpParam = new CallOpParam(true);
            callOpParam.opt.audioCount = 1;
            callOpParam.opt.videoCount = 0;
        }
        public void Dispose()
        {
            hangUp();
            // Dispose managed resources   
            Thread.Sleep(1000);
            callOpParam?.Dispose();
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
        public void hangUp()
        {
            lock (_lock)
            {
                if (activeCall != null)
                {
                    try
                    {
                        RegisterThreadIfNeeded("HangUpThread");
                        activeCall.hangup(callOpParam);
                        Thread.Sleep(500);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error hanging up: " + ex.Message);
                    }
                    finally
                    {
                        activeCall.Dispose();
                        activeCall = null;
                    }
                }
            }
        }
        public void UnRegister()
        {
            hangUp();
            Thread.Sleep(1000);

            Console.WriteLine("*** DESTROYING PJSUA2 ***");
            if (acc != null)
            {
                try
                {
                    acc.setRegistration(false);
                    acc.Dispose();
                    acc = null;

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while unregistering: " + ex.Message);
                }
            }
            if (ep.libGetState() == pjsua_state.PJSUA_STATE_RUNNING)
            {
                try
                {
                    ep.libDestroy();
                    
                    Thread.Sleep(5000);
                    
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while destroying library: " + ex.Message);
                }
            }

        }
        public bool Register(string displayName, string userName, string authenticationId, string registerPassword, string domainHost, string port)
        {
            bool registrationSuccess = false;

            ManualResetEvent regEvent = new ManualResetEvent(false);
            acc.RegistrationStatusChanged += Acc_RegistrationStatusChanged;

            try
            {
                ep.libCreate();
                EpConfig epConfig = new EpConfig();
                epConfig.uaConfig.maxCalls = 1;
                ep.libInit(epConfig);

                TransportConfig tcfg = new TransportConfig();
                tcfg.port = (uint)int.Parse(port);
                ep.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_UDP, tcfg);
                ep.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_TCP, tcfg);
                ep.audDevManager().setNullDev();
                ep.libStart();
                
                Console.WriteLine("*** PJSUA2 STARTED ***");
                ep.codecSetPriority("PCMU/8000", 255);
                ep.codecSetPriority("PCMA/8000", 250);
                
                ep.codecSetPriority("iLBC/8000", 180); // iLBC

                AccountConfig accCfg = new AccountConfig();
                accCfg.idUri = "sip:" + userName + "@" + domainHost;
                accCfg.regConfig.registrarUri = "sip:" + domainHost;
                accCfg.sipConfig.authCreds.Add(new AuthCredInfo("digest", "*", authenticationId, 0, registerPassword));

                Console.WriteLine(accCfg.idUri);

                pjsua_state pjsua_State = ep.libGetState();
                Console.WriteLine(pjsua_State.ToString());

                acc.create(accCfg);
                acc.setRegistration(true);
                Thread.Sleep(5000);

                //acc = new MyAccount { RegEvent = regEvent };
                //acc.create(accCfg);

                //if (regEvent.WaitOne(10000))
                //{
                //    registrationSuccess = acc.RegistrationSuccess;
                //}
                //else
                //{
                //   Console.WriteLine("Registration timed out.");
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return registrationSuccess;
        }
        public void RegisterThreadIfNeeded(string threadName)
        {
            if (!ep.libIsThreadRegistered())
            {
                ep.libRegisterThread(threadName);
            }
        }
        public void makeCall(string ext, string _audioFilePath)
        {
            lock (_lock)
            {
                hangUp();

                if (activeCall != null)
                {
                    activeCall.hangup(callOpParam);
                    activeCall.Dispose();
                    activeCall = null;
                }
                MyCall call = new MyCall(acc, -1, _audioFilePath, this); // Change here
                activeCall = call;
                try
                {
                    if (ep.libGetState() == pjsua_state.PJSUA_STATE_RUNNING)
                    {
                        call.makeCall("sip:" + ext + "@pbx.doorman24.net", callOpParam);
                        Thread.Sleep(1000);
                    }
                    else
                        Console.WriteLine("Register SIP before making call");


                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error making the call: " + ex.Message);
                }
            }
        }
        private void Acc_RegistrationStatusChanged(string status)
        {
            RegisterStatus = status;
        }
        private void Acc_RegistrationStatusChanged(object sender, MyAccount.RegistrationStatusEventArgs e)
        {
            if (e.Status)
            {
                RegisterStatus = "registered";
                Console.WriteLine("Registration successful");
            }
            else
            {
                RegisterStatus = "not registered";
                Console.WriteLine("Registration failed");
            }
        }
        public class MyAccount : Account
        {
            public delegate void RegistrationStatusChangedHandler(object sender, RegistrationStatusEventArgs e);
            public event RegistrationStatusChangedHandler RegistrationStatusChanged;
            public ManualResetEvent RegEvent;
            public bool RegistrationSuccess;
            public class RegistrationStatusEventArgs : EventArgs
            {
                public bool Status { get; set; }
            }
            override public void onRegState(OnRegStateParam prm)
            {
                AccountInfo ai = getInfo();
                Console.WriteLine("***" + (ai.regIsActive ? "" : "Un") + "Register: code=" + prm.code + prm.reason);
                RegistrationStatusChanged?.Invoke(this, new RegistrationStatusEventArgs { Status = ai.regIsActive });
            }
            override public void onIncomingCall(OnIncomingCallParam iprm)
            {
                Call call = new Call(this, iprm.callId);
                CallInfo ci = call.getInfo();
                CallOpParam prm = new CallOpParam();

                Console.WriteLine("*** Incoming Call: " + ci.remoteUri + " [" + ci.stateText + "]" + "\n");
                prm.statusCode = (pjsip_status_code)200;
                call.answer(prm);
            }
        }
        public class MyCall : Call
        {
            private SoftPhone softPhone;
            private MyAccount myAcc;
            private AudioMediaPlayer audioPlayer;
            private string audioFilePath;
            public MyCall(Account acc, int call_id, string audioFilePath, SoftPhone softPhone) : base(acc, call_id) // Modified constructor
            {
                this.audioFilePath = audioFilePath;
                this.softPhone = softPhone; // Assign the passed SoftPhone instance to the new field
                Task.Delay(500);
                myAcc = (MyAccount)acc;
            }
            override public void onCallState(OnCallStateParam prm)
            {
                CallInfo ci = getInfo();
                Console.WriteLine("*** Call: " + ci.remoteUri + " [" + ci.stateText + "], Last status code: " + ci.lastStatusCode + ", Last reason: " + ci.lastReason + "\n");

                if (ci.state == pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED)
                {
                    activeCall = this;

                    PlayAudioFileOverCall(audioFilePath);
                    double audioLengthInSeconds = GetAudioFileDurationInSeconds(audioFilePath);
                    HangUpAfterDelay(audioLengthInSeconds);
                }
                else if (ci.state == pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
                {
                    if (ep.libGetState() == pjsua_state.PJSUA_STATE_RUNNING && audioPlayer != null)
                    {
                        StopAndDisposeAudioPlayer();
                    }
                }
            }
            public void PlayAudioFileOverCall(string audioFilePath)
            {
                Task.Delay(1000);
                AudioMedia play_dev_med = ep.audDevManager().getPlaybackDevMedia();
                Media media = getMedia(0);

                AudioMedia aud_dev = new AudioMedia((System.IntPtr)media.swigCPtr, false);

                // Create the audio media player
                audioPlayer = new AudioMediaPlayer();
                audioPlayer.createPlayer(audioFilePath, 1);
                // Start transmitting the audio file over the call
                audioPlayer.startTransmit(aud_dev);
                Thread.Sleep(300);
                ep.audDevManager().getPlaybackDevMedia().startTransmit(aud_dev);
            }
            public void StopAndDisposeAudioPlayer()
            {
                if (audioPlayer != null)
                {
                    try
                    {
                        // Stop transmitting audio
                        audioPlayer.stopTransmit(ep.audDevManager().getPlaybackDevMedia());

                        // Dispose of the audio player
                        audioPlayer.Dispose();
                        audioPlayer = null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error stopping and disposing audio player: " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("No audio player to stop and dispose.");
                }
            }
            public static double GetAudioFileDurationInSeconds(string path)
            {
                using (var source = CodecFactory.Instance.GetCodec(path))
                {
                    return source.GetLength().TotalSeconds;
                }
            }
            private async void HangUpAfterDelay(double delayInSeconds)
            {
                softPhone.RegisterThreadIfNeeded("HangUpThread");
                await Task.Delay(TimeSpan.FromSeconds(delayInSeconds));
                softPhone.hangUp();

            }
            override public void onCallTransferRequest(OnCallTransferRequestParam prm)
            {
                // Implement your logic here
            }
            override public void onCallReplaceRequest(OnCallReplaceRequestParam prm)
            {
                // Implement your logic here
            }
            public override void onCallMediaState(OnCallMediaStateParam prm)
            {
                CallInfo ci = getInfo();

                for (uint i = 0; i < ci.media.Count; i++)
                {
                    if (ci.media[(int)i].type == pjmedia_type.PJMEDIA_TYPE_AUDIO && getStreamInfo(i) != null)
                    {

                        // Get the Media for the stream index

                    }
                }
            }



        }

    }
}
