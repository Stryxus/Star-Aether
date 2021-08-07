﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Net.WebSockets;

namespace UEESA.Shared.Sockets
{
    public class ConnectionManager<T> where T : WebSocket
    {
        private ConcurrentDictionary<Guid, T> socketDictionary = new ConcurrentDictionary<Guid, T>();

        public T GetSocketById(Guid id) => socketDictionary.FirstOrDefault(p => p.Key == id).Value;

        public ConcurrentDictionary<Guid, T> GetAll() => socketDictionary;

        public Guid GetId(T socket) => socketDictionary.FirstOrDefault(p => p.Value == socket).Key;

        public void AddSocket(T socket) => socketDictionary.TryAdd(Guid.NewGuid(), socket);

        public async Task RemoveSocket(Guid id)
        {
            socketDictionary.TryRemove(id, out T socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Socket Closed.", CancellationToken.None);
            socket.Dispose();
        }
    }
}
