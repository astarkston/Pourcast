#define DEBUG_LEVEL 5

class NetworkRequester  {
  public:
    NetworkRequester(const char* host, int port, byte pin);
    void MakeRequest(String url);
    void LogMessage(String message);
    void Heartbeat();
  private:
    const char* _host;
    int _port;
    byte _pin;
};