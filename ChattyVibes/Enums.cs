namespace ChattyVibes
{
    enum ConnectionState
    {
        NotConnected,
        Connecting,
        Connected,
        Disconnecting,
        Error
    }

    enum QueuedItemType
    {
        Message,
        Whisper,
        NewSub,
        GiftSub,
        ContGiftSub,
        ComSub,
        PrimeSub,
        ReSub
    }
}
