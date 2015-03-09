using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatSharp
{
    public class RequestManager
    {
        public RequestManager()
        {
            PendingOperations = new Dictionary<string, RequestOperation>();
        }

        private Dictionary<string, RequestOperation> PendingOperations { get; set; }

        public void QueueOperation(string key, RequestOperation operation)
        {
            if (PendingOperations.ContainsKey(key))
                throw new InvalidOperationException("Operation is already pending.");
            PendingOperations.Add(key, operation);
        }

        public RequestOperation PeekOperation(string key)
        {
            var realKey = PendingOperations.Keys.FirstOrDefault(k => string.Compare(k, key, StringComparison.OrdinalIgnoreCase) == 0);
            return PendingOperations[realKey];
        }

        public RequestOperation DequeueOperation(string key)
        {
            var operation = PendingOperations[key];
            PendingOperations.Remove(key);
            return operation;
        }
    }

    public class RequestOperation
    {
        public object State { get; set; }
        public Action<RequestOperation> Callback { get; set; }

        public RequestOperation(object state, Action<RequestOperation> callback)
        {
            State = state;
            Callback = callback;
        }
    }
}
