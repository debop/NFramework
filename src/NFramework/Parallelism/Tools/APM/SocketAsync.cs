using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.Tools {
    public static class SocketAsync {
        public static Task<Socket> AcceptAsync(this Socket socket) {
            return Task<Socket>.Factory.FromAsync(socket.BeginAccept,
                                                  socket.EndAccept,
                                                  null);
        }

        public static Task ConnectAsync(this Socket socket, EndPoint remoteEP) {
            return
                Task.Factory.FromAsync(socket.BeginConnect,
                                       socket.EndConnect,
                                       remoteEP,
                                       null);
        }

        public static Task ConnectAsync(this Socket socket, IPAddress address, int port) {
            return
                Task.Factory.FromAsync(socket.BeginConnect,
                                       socket.EndConnect,
                                       address,
                                       port,
                                       null);
        }

        public static Task<int> ReceiveAsync(this Socket socket,
                                             byte[] buffer,
                                             int offset = 0,
                                             int size = 0,
                                             SocketFlags socketFlags = SocketFlags.None) {
            if(size <= 0)
                size = buffer.Length;

            var tcs = new TaskCompletionSource<int>();
            socket.BeginReceive(buffer, offset, size, socketFlags,
                                iar => {
                                    try {
                                        tcs.TrySetResult(socket.EndReceive(iar));
                                    }
                                    catch(Exception ex) {
                                        tcs.TrySetException(ex);
                                    }
                                }, null);

            return tcs.Task;
        }

        public static Task<int> SendAsync(this Socket socket,
                                          byte[] buffer,
                                          int offset = 0,
                                          int size = 0,
                                          SocketFlags socketFlags = SocketFlags.None) {
            if(size <= 0)
                size = buffer.Length;

            var tcs = new TaskCompletionSource<int>();
            socket.BeginSend(buffer, offset, size, socketFlags,
                             iar => {
                                 try {
                                     tcs.TrySetResult(socket.EndSend(iar));
                                 }
                                 catch(Exception ex) {
                                     tcs.TrySetException(ex);
                                 }
                             }, null);

            return tcs.Task;
        }
    }
}