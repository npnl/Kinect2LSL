using System;
using System.Collections.Generic;
using LSL;
using Microsoft.Kinect;
using System.Threading;

namespace Kinect2lsl
{
    class Program
    {

        private KinectSensor _sensor;
        private MultiSourceFrameReader _reader;
        private IList<Body> _bodies;
        private String kinectLimb;
        private liblsl.StreamOutlet KINECToutlet;
        private Boolean streaming;

        public static void Main(string[] args)
        {

            Program p = new Program();
            p.SendKinectStream("spine", 9);
            Console.ReadLine();
            p.streaming = false;
        }


        public void SendKinectStream(String limb, Int32 channels)
        {
            kinectLimb = limb;
            liblsl.StreamInfo KINECTinfo = new liblsl.StreamInfo("KINECT" + "_raw", "KINECTpos_raw", channels, 24, liblsl.channel_format_t.cf_float32, "Microsoft");
            KINECToutlet = new liblsl.StreamOutlet(KINECTinfo);
            streaming = true;
            Thread thisThread = new Thread(ReadKinect)
            {
                IsBackground = true,
                Name = "threadKinect"
            };
            thisThread.Start();
            
        }

        private void ReadKinect()
        {
            //while (streaming)
            //{
                _sensor = KinectSensor.GetDefault();

                if (_sensor != null)
                //if (_sensor.IsAvailable)
                {
                    //Console.WriteLine("Streaming Kinect to LSL.\nPress any key to finish...");
                    _sensor.Open();

                    _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                    _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
                }
                else
                {
                    Console.WriteLine("Couldn't find Kinect sensor.\nPress any key to finish...");
                    //break;
                }
            //}
        }

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();
            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    _bodies = new Body[frame.BodyFrameSource.BodyCount];

                    frame.GetAndRefreshBodyData(_bodies);

                    foreach (var body in _bodies)
                    {
                        if (body != null)
                        {
                            if (body.IsTracked)
                            {
                                Joint spineBase = body.Joints[JointType.SpineBase];
                                Joint spineMid = body.Joints[JointType.SpineMid];
                                Joint spineShoulder = body.Joints[JointType.SpineShoulder];
                                Joint shoulderLeft = body.Joints[JointType.ShoulderLeft];
                                Joint elbowLeft = body.Joints[JointType.ElbowLeft];
                                Joint wristLeft = body.Joints[JointType.WristLeft];
                                Joint handLeft = body.Joints[JointType.HandLeft];
                                Joint thumbLeft = body.Joints[JointType.ThumbLeft];
                                Joint shoulderRight = body.Joints[JointType.ShoulderRight];
                                Joint elbowRight = body.Joints[JointType.ElbowRight];
                                Joint wristRight = body.Joints[JointType.WristRight];
                                Joint handRight = body.Joints[JointType.HandRight];
                                Joint thumbRight = body.Joints[JointType.ThumbRight];

                                float[,] KINECTdata;

                                switch (kinectLimb)
                                {
                                    case "trunkLeft":
                                        KINECTdata = new float[1, 21] { {
                                                spineBase.Position.X, spineBase.Position.Y, spineBase.Position.Z,
                                                spineMid.Position.X, spineMid.Position.Y, spineMid.Position.Z,
                                                spineShoulder.Position.X, spineShoulder.Position.Y, spineShoulder.Position.Z,
                                                shoulderLeft.Position.X, shoulderLeft.Position.Y, shoulderLeft.Position.Z,
                                                elbowLeft.Position.X, elbowLeft.Position.Y, elbowLeft.Position.Z,
                                                wristLeft.Position.X, wristLeft.Position.Y, wristLeft.Position.Z,
                                                handLeft.Position.X, handLeft.Position.Y, handLeft.Position.Z
                                            } };
                                        KINECToutlet.push_chunk(KINECTdata);
                                        break;
                                    case "trunkRight":
                                        KINECTdata = new float[1, 21] { {
                                                spineBase.Position.X, spineBase.Position.Y, spineBase.Position.Z,
                                                spineMid.Position.X, spineMid.Position.Y, spineMid.Position.Z,
                                                spineShoulder.Position.X, spineShoulder.Position.Y, spineShoulder.Position.Z,
                                                shoulderRight.Position.X, shoulderRight.Position.Y, shoulderRight.Position.Z,
                                                elbowRight.Position.X, elbowRight.Position.Y, elbowRight.Position.Z,
                                                wristRight.Position.X, wristRight.Position.Y, wristRight.Position.Z,
                                                handRight.Position.X, handRight.Position.Y, handRight.Position.Z
                                            } };
                                        KINECToutlet.push_chunk(KINECTdata);
                                        break;
                                    case "armLeft":
                                        KINECTdata = new float[1, 18] { {
                                                spineShoulder.Position.X, spineShoulder.Position.Y, spineShoulder.Position.Z,
                                                shoulderLeft.Position.X, shoulderLeft.Position.Y, shoulderLeft.Position.Z,
                                                elbowLeft.Position.X, elbowLeft.Position.Y, elbowLeft.Position.Z,
                                                wristLeft.Position.X, wristLeft.Position.Y, wristLeft.Position.Z,
                                                handLeft.Position.X, handLeft.Position.Y, handLeft.Position.Z,
                                                thumbLeft.Position.X, thumbLeft.Position.Y, thumbLeft.Position.Z
                                            } };
                                        KINECToutlet.push_chunk(KINECTdata);
                                        break;
                                    case "armRight":
                                        KINECTdata = new float[1, 18] { {
                                                spineShoulder.Position.X, spineShoulder.Position.Y, spineShoulder.Position.Z,
                                                shoulderRight.Position.X, shoulderRight.Position.Y, shoulderRight.Position.Z,
                                                elbowRight.Position.X, elbowRight.Position.Y, elbowRight.Position.Z,
                                                wristRight.Position.X, wristRight.Position.Y, wristRight.Position.Z,
                                                handRight.Position.X, handRight.Position.Y, handRight.Position.Z,
                                                thumbRight.Position.X, thumbRight.Position.Y, thumbRight.Position.Z
                                            } };
                                        KINECToutlet.push_chunk(KINECTdata);
                                        break;
                                    case "spine":
                                        KINECTdata = new float[1, 9] { { spineBase.Position.X, spineBase.Position.Y, spineBase.Position.Z,
                                            spineMid.Position.X, spineMid.Position.Y, spineMid.Position.Z,
                                            spineShoulder.Position.X, spineShoulder.Position.Y, spineShoulder.Position.Z} };
                                        //Debug.WriteLine("SpineShoulderX: " + KINECTdata[0,6] + "; SpineShoulderY: " + KINECTdata[0, 7] + "; SpineShoulderZ: " + KINECTdata[0, 8]);
                                        KINECToutlet.push_chunk(KINECTdata);
                                        break;
                                    default:
                                        KINECTdata = new float[1, 39] { {
                                                spineBase.Position.X, spineBase.Position.Y, spineBase.Position.Z,
                                                spineMid.Position.X, spineMid.Position.Y, spineMid.Position.Z,
                                                spineShoulder.Position.X, spineShoulder.Position.Y, spineShoulder.Position.Z,
                                                shoulderLeft.Position.X, shoulderLeft.Position.Y, shoulderLeft.Position.Z,
                                                elbowLeft.Position.X, elbowLeft.Position.Y, elbowLeft.Position.Z,
                                                wristLeft.Position.X, wristLeft.Position.Y, wristLeft.Position.Z,
                                                handLeft.Position.X, handLeft.Position.Y, handLeft.Position.Z,
                                                thumbLeft.Position.X, thumbLeft.Position.Y, thumbLeft.Position.Z,
                                                shoulderRight.Position.X, shoulderRight.Position.Y, shoulderRight.Position.Z,
                                                elbowRight.Position.X, elbowRight.Position.Y, elbowRight.Position.Z,
                                                wristRight.Position.X, wristRight.Position.Y, wristRight.Position.Z,
                                                handRight.Position.X, handRight.Position.Y, handRight.Position.Z,
                                                thumbRight.Position.X, thumbRight.Position.Y, thumbRight.Position.Z
                                            } };
                                        KINECToutlet.push_chunk(KINECTdata);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
