namespace Assignment.DosProtection.DM
{
    public class KeySignalEventArgs : EventArgs
    {
        public string Key { get; }

        public KeySignalEventArgs(string key)
        {
            Key = key;
        }
    }

    public class KeySignalEvent
    {
        public event EventHandler<KeySignalEventArgs> HttpRequestReceived;

        public void OnHttpRequestReceived(string key)
        {
            HttpRequestReceived?.Invoke(this, new KeySignalEventArgs(key));
        }
    }
}
