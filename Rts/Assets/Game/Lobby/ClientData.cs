namespace Game.Lobby
{
    [System.Serializable]
    public class ClientData
    {
        public string Name;
        public bool Ready;
        public short Token { get; private set; }

        public ClientData(string name, short token)
        {
            this.Name = name;
            this.Token = token;
        }
    }
}
