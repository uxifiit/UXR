//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using UXR.Models.Entities;
//using UXR.Studies.Models;
//using System.Collections.Generic;
//using UXR.Studies.Api.Controllers;
//using UXR.Common;
//using UXR.Studies.Api.Entities.Sessions;
//using System.Linq;
//using System.Linq.Expressions;
//using AutoMapper;
//using UXR.Studies.Api.MapperProfiles;

//namespace UXR.Studies.Api.Test
//{
//    [TestClass]
//    public class SessionApiProfileTest
//    {
//        private readonly string PROJECT_NAME = "Test project";

//        private readonly int SESSION1_ID = 1;
//        private readonly string SESSION1_NAME = "Test session 1";
//        private readonly DateTime SESSION1_START_TIME = new DateTime(2017, 5, 4, 12, 0, 0);
//        private readonly TimeSpan SESSION1_LENGTH = TimeSpan.FromHours(2.5);

//        private readonly int SESSION2_ID = 2;
//        private readonly string SESSION2_NAME = "Test session 2";
//        private readonly DateTime SESSION2_START_TIME = new DateTime(2017, 5, 5, 14, 0, 0);
//        private readonly TimeSpan SESSION2_LENGTH = TimeSpan.FromHours(3);

//        private readonly string OWNER_NAME = "user@stuba.sk";

//        private readonly string STREAM_TYPE1_NAME = "Screencast";
//        private readonly string STREAM_TYPE1_OPTION = "HD";
//        private readonly string STREAM_TYPE1_TYPECODE = "SCV";

//        private readonly string STREAM_TYPE2_NAME = "Eyetracker";
//        private readonly string STREAM_TYPE2_TYPECODE = "ET";


//        [TestMethod]
//        public void Test_MapsSessionDataCorrectly()
//        {
//            var config = new MapperConfiguration(cfg =>
//            {
//                cfg.AddProfile<SessionApiProfile>();
//            });

//            var mapper = config.CreateMapper();

//            var project = new Project()
//            {
//                Name = PROJECT_NAME,
//                Owner = new ApplicationUser()
//                {
//                    UserName = OWNER_NAME,
//                },
//                SessionTemplate = "{ }"
//                //StreamSettings = new List<ProjectStreamSetting>
//                //        {
//                //            new ProjectStreamSetting()
//                //            {
//                //                StreamType = new StreamType()
//                //                {
//                //                    Name = STREAM_TYPE1_NAME,
//                //                    TypeCode = STREAM_TYPE1_TYPECODE
//                //                },
//                //                StreamTypeOption = new StreamTypeOption()
//                //                {
//                //                    Name = STREAM_TYPE1_OPTION
//                //                }
//                //            },
//                //            new ProjectStreamSetting()
//                //            {
//                //                StreamType = new StreamType()
//                //                {
//                //                    Name = STREAM_TYPE2_NAME,
//                //                    TypeCode = STREAM_TYPE2_TYPECODE
//                //                }
//                //            }
//                //        }
//            };

//            var activeSessions = new List<Session>()
//            {
//                new Session()
//                {
//                    Id = SESSION1_ID,
//                    Name = SESSION1_NAME,
//                    StartTime = SESSION1_START_TIME,
//                    Length = SESSION1_LENGTH,
//                    Project = project,
//                    Definition = "{ }"
//                },
//                new Session()
//                {
//                    Id = SESSION2_ID,
//                    Name = SESSION2_NAME,
//                    StartTime = SESSION2_START_TIME,
//                    Length = SESSION2_LENGTH,
//                    Project = project,
//                    Definition = "{ }"
//                }
//            };



//            var sessionInfoList = activeSessions.Select(mapper.Map<SessionInfo>).ToList();

//            Assert.IsNotNull(sessionInfoList);
//            Assert.IsTrue(sessionInfoList.Count == 2);

//            Assert.AreEqual(SESSION1_ID, sessionInfoList[0].Id);
//            Assert.AreEqual(SESSION1_NAME, sessionInfoList[0].Name);
//            Assert.AreEqual(SESSION1_START_TIME, sessionInfoList[0].StartTime);
//            Assert.AreEqual(SESSION1_LENGTH.TotalMinutes, sessionInfoList[0].LengthTotalMinutes);
//            Assert.AreEqual(PROJECT_NAME, sessionInfoList[0].Project);
//            Assert.AreEqual(OWNER_NAME, sessionInfoList[0].Owner);

//            Assert.AreEqual(SESSION2_ID, sessionInfoList[1].Id);
//            Assert.AreEqual(SESSION2_NAME, sessionInfoList[1].Name);
//            Assert.AreEqual(SESSION2_START_TIME, sessionInfoList[1].StartTime);
//            Assert.AreEqual(SESSION2_LENGTH.TotalMinutes, sessionInfoList[1].LengthTotalMinutes);
//            Assert.AreEqual(PROJECT_NAME, sessionInfoList[1].Project);
//            Assert.AreEqual(OWNER_NAME, sessionInfoList[1].Owner);

//            Assert.IsNotNull(sessionInfoList[0].StreamSettings);
//            Assert.AreEqual(STREAM_TYPE1_TYPECODE, sessionInfoList[0].StreamSettings[0].TypeCode);
//            Assert.AreEqual(STREAM_TYPE1_OPTION, sessionInfoList[0].StreamSettings[0].Option);
//            Assert.AreEqual(STREAM_TYPE2_TYPECODE, sessionInfoList[0].StreamSettings[1].TypeCode);
//            Assert.IsNull(sessionInfoList[0].StreamSettings[1].Option);
//        }

//    }
//}
