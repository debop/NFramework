using System;
using System.Collections.Generic;
using System.Messaging;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.WindowsSystem {
    /// <summary>
    /// Microsoft Message Queue에 대한 Utility Class 입니다.
    /// </summary>
    /// <remark>
    /// <b>ActiveX Library와는 달리 .NET Framework Class이므로 이를 생성해서 사용하는 계정의 Windows System 권한이 중요하다.</b>
    /// </remark>
    public static class MsmqTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Private MessageQueue의 Root Path
        /// </summary>
        public const string PRIVATE_PATH = @".\private$\";

        /// <summary>
        /// 가동 시스템의 공개 큐들을 1차원 배열로 반환한다.
        /// </summary>
        /// <returns>큐의 1차원 배열</returns>
        public static MessageQueue[] GetPublicQueues() {
            return GetPublicQueues(Environment.MachineName);
        }

        /// <summary>
        /// 가동 시스템의 공개 큐들을 지정된 리스트에 추가한다.
        /// </summary>
        /// <param name="list">큐를 저장할 리스트 객체</param>
        public static void GetPublicQueues(List<MessageQueue> list) {
            GetPublicQueues(Environment.MachineName, list);
        }

        /// <summary>
        /// 지정된 머신의 공개 큐들을 1차원 배열로 반환한다.
        /// </summary>
        /// <param name="machineName">컴퓨터 이름</param>
        /// <returns>큐의 배열, 실패시에는 길이가 0인 배열</returns>
        public static MessageQueue[] GetPublicQueues(string machineName) {
            if(machineName.IsWhiteSpace())
                machineName = Environment.MachineName;

            return MessageQueue.GetPublicQueuesByMachine(machineName);
            //MessageQueue[] mqs = MessageQueue.GetPublicQueuesByMachine(machineName);
            //return (mqs != null) ? mqs : new MessageQueue[0];
        }

        /// <summary>
        /// 가동 시스템의 공개 큐들을 지정된 리스트에 추가한다.
        /// </summary>
        /// <param name="machineName">컴퓨터 이름</param>
        /// <param name="list">큐 리스트</param>
        public static void GetPublicQueues(string machineName, List<MessageQueue> list) {
            list.ShouldNotBeNull("list");
            var mqs = GetPublicQueues(machineName);

            list.AddRange(mqs);
        }

        /// <summary>
        /// 현 시스템의 비공개 큐들을 1차원 배열로 반환한다.
        /// </summary>
        /// <returns></returns>
        public static MessageQueue[] GetPrivateQueues() {
            return GetPrivateQueues(Environment.MachineName);
        }

        /// <summary>
        /// 지정된 컴퓨터이름을 가진 시스템의 비공개 큐들을 1차원 배열로 반환한다.
        /// </summary>
        /// <remarks>
        /// 타 컴퓨터의 경우 보안이 제대로 설정되어 있지 않다면, 실패할 것이다.
        /// </remarks>
        /// <param name="machineName"></param>
        /// <returns>1차원 배열</returns>
        public static MessageQueue[] GetPrivateQueues(string machineName) {
            if(machineName.IsWhiteSpace())
                machineName = Environment.MachineName;

            if(IsDebugEnabled)
                log.Debug("PrivateQueue 를 조회합니다. machineName=[{0}]", machineName);

            return MessageQueue.GetPrivateQueuesByMachine(machineName);
        }

        /// <summary>
        /// 현 시스템의 비공개 큐들을 제공된 리스트에 추가한다.
        /// </summary>
        /// <param name="list">큐를 저장할 리스트 객체</param>
        public static void GetPrivateQueue(List<MessageQueue> list) {
            GetPrivateQueues(Environment.MachineName, list);
        }

        /// <summary>
        /// 지정된 컴퓨터이름을 가진 시스템의 비공개 큐들을 제공된 리스트에 추가한다.
        /// </summary>
        /// <remarks>
        /// 타 컴퓨터의 경우 보안이 제대로 설정되어 있지 않다면, 실패할 것이다.
        /// </remarks>
        /// <param name="machineName">큐가 있는 컴퓨터이름</param>
        /// <param name="list">큐를 저장할 리스트 객체</param>
        public static void GetPrivateQueues(string machineName, List<MessageQueue> list) {
            var mqs = GetPrivateQueues(machineName);
            list.AddRange(mqs);
        }

        /// <summary>
        /// 메시지 큐가 지정된 경로에 존재하는지 확인합니다.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Exists(string path) {
            if(path.IsEmpty())
                return false;

            return MessageQueue.Exists(path);
        }

        /// <summary>
        /// 새로운 개인 대기열을 생성합니다.
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public static MessageQueue CreatePrivateQueue(string queueName) {
            return CreatePrivateQueue(queueName, string.Empty, false);
        }

        /// <summary>
        /// 개인 대기열을 생성합니다.
        /// </summary>
        /// <param name="queueName">대기열 이름</param>
        /// <param name="label">대기열 라벨</param>
        /// <returns>생성된 대기열</returns>
        public static MessageQueue CreatePrivateQueue(string queueName, string label) {
            return CreatePrivateQueue(queueName, label, false);
        }

        /// <summary>
        /// 개인 대기열을 생성합니다.
        /// </summary>
        /// <param name="queueName">개인대기열 이름</param>
        /// <param name="transactional">Transaction 지원 유무</param>
        /// <returns>생성된 개인 대기열</returns>
        public static MessageQueue CreatePrivateQueue(string queueName, bool transactional) {
            return CreatePrivateQueue(queueName, string.Empty, transactional);
        }

        /// <summary>
        /// 새로운 개인 대기열을 생성합니다.
        /// </summary>
        /// <param name="queueName">개인대기열 이름</param>
        /// <param name="label">라벨</param>
        /// <param name="transactional">Transaction 지원 유무</param>
        /// <returns>생성된 개인 대기열</returns>
        public static MessageQueue CreatePrivateQueue(string queueName, string label, bool transactional) {
            return CreateQueue(PRIVATE_PATH + queueName, label, transactional);
        }

        /// <summary>
        /// 지정된 경로에 공개 메시지 대기열을 만듭니다.  
        /// </summary>
        /// <param name="path">대기열 경로</param>
        /// <returns>생성된 Message Queue</returns>
        public static MessageQueue CreateQueue(string path) {
            return CreateQueue(path, string.Empty, false);
        }

        /// <summary>
        /// 지정된 경로에 공개 메시지 대기열을 만듭니다.  
        /// </summary>
        /// <param name="path">대기열 경로</param>
        /// <param name="queueLabel">대기열 라벨</param>
        /// <returns>생성된 Message Queue</returns>
        public static MessageQueue CreateQueue(string path, string queueLabel) {
            return CreateQueue(path, string.Empty, false);
        }

        /// <summary>
        /// 지정된 경로에 공개 메시지 대기열을 만듭니다.  
        /// </summary>
        /// <param name="path">대기열 경로</param>
        /// <param name="transactional">Transaction 지원 유무</param>
        /// <returns>생성된 Message Queue</returns>
        public static MessageQueue CreateQueue(string path, bool transactional) {
            return CreateQueue(path, string.Empty, transactional);
        }

        /// <summary>
        /// 지정된 경로에 공개 메시지 대기열을 만듭니다.  
        /// </summary>
        /// <param name="path">대기열 경로</param>
        /// <param name="label">대기열 라벨</param>
        /// <param name="transactional">Transaction 지원 유무</param>
        /// <returns>생성된 Message Queue, 생성에 실패하념 null이 반환된다.</returns>
        public static MessageQueue CreateQueue(string path, string label, bool transactional) {
            path.ShouldNotBeWhiteSpace("path");

            if(IsDebugEnabled)
                log.Debug("새로운 큐를 생성합니다... path=[{0}], label=[{1}], transactional=[{2}]", path, label, transactional);

            MessageQueue q;

            if(Exists(path) == false) {
                q = MessageQueue.Create(path, transactional);

                if(label.IsNotEmpty()) {
                    try {
                        q.Label = label;
                    }
                    catch(Exception ex) {
                        if(log.IsWarnEnabled) {
                            log.Warn("큐의 Label을 설정하는데 실패했습니다!!! label=[{0}]", label);
                            log.Warn(ex);
                        }
                    }
                }
            }
            else {
                q = new MessageQueue(path);
            }

            if(log.IsInfoEnabled)
                log.Info("새로운 큐를 생성했습니다!!! path=[{0}], label=[{1}], transactional=[{2}]", path, label, transactional);

            return q;
        }

        /// <summary>
        /// message body를 인자로 대기열에 전송
        /// </summary>
        /// <param name="path">보낼 대기열 경로</param>
        /// <param name="body">보낼 내용</param>
        public static void Send(string path, object body) {
            path.ShouldNotBeWhiteSpace("path");
            Send(path, body, string.Empty);
        }

        /// <summary>
        /// message body를 인자로 대기열에 전송
        /// </summary>
        /// <param name="path">보낼 대기열 경로</param>
        /// <param name="body">보낼 내용</param>
        /// <param name="label">메시지 라벨</param>
        public static void Send(string path, object body, string label) {
            path.ShouldNotBeWhiteSpace("path");
            Send(path, body, label, 0);
        }

        /// <summary>
        /// System.Messaging.Message 객체를 Message Queue에 전송한다.
        /// </summary>
        /// <param name="path">전송할 대기열 위치</param>
        /// <param name="body">본문 내용</param>
        /// <param name="label">메시지 라벨</param>
        /// <param name="appSpec">메시지 구분을 위한 Message AppSpecific</param>
        public static void Send(string path, object body, string label, int appSpec) {
            path.ShouldNotBeWhiteSpace("path");

            if(IsDebugEnabled)
                log.Debug("큐에 메시지를 전송합니다... queue path=[{0}], body=[{1}], label=[{2}], appSpec=[{3}]", path, body, label, appSpec);

            using(var msg = new Message(body)) {
                msg.Label = label;
                msg.AppSpecific = appSpec;

                SendMessage(path, msg);
            }
        }

        /// <summary>
        /// System.Messaging.Message 형식의 객체를 지정된 대기열에 전송한다.
        /// </summary>
        /// <param name="path">지정된 대기열 위치</param>
        /// <param name="message">전송할 메시지 객체</param>
        public static void SendMessage(string path, Message message) {
            path.ShouldNotBeWhiteSpace("path");

            if(IsDebugEnabled)
                log.Debug("큐에 메시지를 전송합니다.. queue path=[{0}], message=[{1}]", path, message);

            if(message == null)
                return;

            using(var q = CreateQueue(path)) {
                q.Send(message);

                if(IsDebugEnabled)
                    log.Debug("큐에 메시지 전송을 완료했습니다.");
            }
        }

        /// <summary>
        /// 지정된 대기열에 있는 모든 메시지의 복사본을 가져온다.
        /// </summary>
        /// <param name="path">지정된 대기열</param>
        /// <returns>지정된 대기열에 있는 모든 메시지</returns>
        public static Message[] GetAllMessages(string path) {
            path.ShouldNotBeWhiteSpace("path");

            if(IsDebugEnabled)
                log.Debug("지정한 큐의 모든 메시지를 읽어옵니다. path=[{0}]", path);

            using(MessageQueue q = CreateQueue(path))
                return (q != null) ? q.GetAllMessages() : new Message[0];
        }

        /// <summary>
        /// 지정된 대기열에서 foreach 구문을 위한 <c>System.Messaging.MessageEnumerator</c> 개체를 반환<br/>
        /// 기본적으로 대기열안의 본문 내용이 문자열로 가정한다.
        /// </summary>
        /// <param name="path">지정된 대기열 위치</param>
        /// <returns><c>System.Messaging.MessageEnumerator</c> 객체</returns>
        public static MessageEnumerator GetMessageEnumerator(string path) {
            return GetMessageEnumerator(path, new[] { typeof(string) });
        }

        /// <summary>
        /// 지정된 대기열에서 foreach 구문을 위한 <c>System.Messaging.MessageEnumerator</c> 개체를 반환<br/>
        /// <b>개체(사용자가 정의한 일반 Class) 를 사용할 수도 있도록 Type[] 개체를 받음</b>
        /// <remarks>
        ///	이때의 Type은 <c>XmlMessageFormatter</c>를 이용하여
        ///	XML로 serialize/deserialize되므로 <c>SerializableAttribute</c> 특성이 지정되었거나 
        ///	<c>ISerializable</c> 인터페이스를 구현한 Class여야 한다.
        /// </remarks>
        /// </summary>
        /// <example>
        /// 
        /// <code>
        /// // RwMqUtilityTest.cs 파일의 ReadMessages Method 참조
        /// MessageEnumerator me = MsmqUtil.GetMessageEnumerator(path);//, new Type[] { typeof(string) } );
        /// Message m;
        /// 
        /// Console.WriteLine("Label       ID      BODY");
        /// while (me.MoveNext())
        /// {
        ///     m = me.Current;
        /// 
        ///     Console.WriteLine("{0} {1} {2}", m.Label, m.Id, m.Body);
        ///     me.RemoveCurrent();
        /// }
        /// me.Dispose();
        /// </code>
        /// </example>
        /// <param name="path">지정된 대기열 위치</param>
        /// <param name="types">XmlMessageFormatter 형식</param>
        /// <returns>foreach문을 지원하기 위한<c>MessageEnumerator</c> 객체</returns>
        public static MessageEnumerator GetMessageEnumerator(string path, Type[] types) {
            path.ShouldNotBeWhiteSpace("path");

            if(IsDebugEnabled)
                log.Debug("MessageEnumerator를 얻어옵니다... path=[{0}], types=[{1}]", path, types.CollectionToString());

            MessageEnumerator me = null;
            using(var q = CreateQueue(path)) {
                if(q != null) {
                    q.Formatter = new XmlMessageFormatter(types);
                    me = q.GetMessageEnumerator2();
                }
            }
            return me;
        }
    }
}