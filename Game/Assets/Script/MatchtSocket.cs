using Socket.Quobject.SocketIoClientDotNet.Client;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchtSocket : MonoBehaviour
{
    public QSocket socket;
    private string id;
    public string opponent_id = "";
    public string opponent_nick = "";
    string match_id = "";
    public GameManager manager;

    public int numPLayer;
    int round = 0;

    public bool sorted = false;

    bool opponentPlaced = false;
    public string opponentPlacedList = "";
    int lastOpponentRound = 0;
    string action = "";
    int fromPos = 0;
    int toPos = 0;
    bool notFoundSwitched = false;

    public string local_action = "";
    public int local_fromPos = 0;
    public int local_toPos = 0;
    public bool local_notFoundSwitched = false;

    char firstPlayer = '-';

    string errorText = "";
    bool showError = false;

    string private_code = "";
    bool private_created = false;

    float timer = 0.0f;
    int seconds = 0;
    string opponent_left_msg = "YOU WIN!\nThe opponent left";
    public bool stop = false;

    public GameObject errorTxt;

    //Start is called before the first frame update
    void Start() {
        manager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        this.id = PlayerPrefs.GetString("player-uuid");
    }

    // Update is called once per frame
    void Update()
    {
        if (!stop)
        {
            if (private_created)
            {
                manager.onMultiPrivateCreated(private_code);
                private_created = false;
            }

            if (showError)
            {
                if (errorText == opponent_left_msg)
                    emitMatchEnd("win");

                manager.waitingPanelServer.SetActive(false);
                manager.showAlert(errorText);
                showError = false;
            }

            if(manager!=null && opponent_id!="")
            {
                if (firstPlayer != '-')
                {
                    manager.hideAlert();
                    manager.roundPlayer = firstPlayer;
                    manager.setInfoText("Placement Round!");
                    manager.opponentReady = true;
                    manager.setOpponentNick(opponent_nick);
                    firstPlayer = '-';
                }

                if (!opponentPlaced && opponentPlacedList.Length == 18)
                {//Attende il posizionamento delle carte
                    Debug.Log("START");
                    manager.multiPLaceOpponentCards(opponentPlacedList);
                    opponentPlaced = true;
                    manager.cardPlaced = true;
                }

                if (manager.round < round)
                {//nuovo round
                    Debug.Log(action);
                    if (action.Equals("equip-boost"))
                        manager.multiEquiBoost(fromPos);

                    if (action.Equals("disable-boost"))
                        manager.multiDisableBoost();

                    if (action.Equals("mouve"))
                        manager.multiMouve(fromPos, toPos);

                    if (action.Equals("exit"))
                        manager.multiExit(fromPos);

                    if (action.Equals("active-viruscheck"))
                        manager.multiVirusChecker(fromPos);

                    if (action.Equals("active-reset"))
                        manager.multiReset(fromPos);

                    if (action.Equals("active-404notfound"))
                        manager.multiNotFound(fromPos, toPos, notFoundSwitched);

                    if (action.Equals("enable-firewall"))
                        manager.multiEnableFirewall(fromPos);

                    if (action.Equals("disable-firewall"))
                        manager.multiDisableFirewall(fromPos);

                    lastOpponentRound = round;
                    manager.nextRound();

                }
            }  
        }
        
    }

    public void connect()
    {
        IO.Options options = new IO.Options();
        options.Transports = Socket.Quobject.Collections.Immutable.ImmutableList.Create<string>("websocket");
        socket = IO.Socket(NetInfo.MATCHMAKING_SERVER, options);

        socket.On(QSocket.EVENT_CONNECT, () =>
        {
            Debug.Log("Socket connected!");
        });

        socket.On(QSocket.EVENT_ERROR, (reason) =>
        {
            Debug.Log(reason);
        });

        socket.On("ready", (response) =>
        {
            Debug.Log("Match start!");
            JSONObject x = new JSONObject(response.ToString());
            opponent_id = (x.GetField("opponent").ToString()).Replace("\"", string.Empty);
            opponent_nick = (x.GetField("opponent_nick").ToString()).Replace("\"", string.Empty);
            match_id = (x.GetField("match_id").ToString()).Replace("\"", string.Empty);

            if (x.GetField("roundPlayer").ToString().Contains("A"))
                firstPlayer = 'A';
            else
                firstPlayer = 'B';

        });

        socket.On("update_place", (response) =>
        {
            JSONObject x = new JSONObject(response.ToString());
            opponentPlacedList = (x.GetField("placed_list").ToString()).Replace("\"", string.Empty);
        });


        socket.On("left", (response) =>
        {
            errorText = opponent_left_msg;
            showError = true;
        });

        socket.On("next_round", (response) =>
        {
            JSONObject x = new JSONObject(response.ToString());
            action = (x.GetField("action").ToString()).Replace("\"", string.Empty);
            fromPos = Int32.Parse((x.GetField("from_pos").ToString()).Replace("\"", string.Empty));
            toPos = Int32.Parse((x.GetField("to_pos").ToString()).Replace("\"", string.Empty));
            notFoundSwitched = Convert.ToBoolean((x.GetField("nfs").ToString()).Replace("\"", string.Empty));
            round = Int32.Parse((x.GetField("round").ToString()).Replace("\"", string.Empty));

        });

        socket.On("NF", (response) =>
        {
            errorText = "Match not found\nWrong code?";
            showError = true;

        });

        socket.On("private_room_created", (response) =>
        {
            JSONObject x = new JSONObject(response.ToString());
            private_code = (x.GetField("code").ToString()).Replace("\"", string.Empty);
            private_created = true;

        });

    }

    public void nextRouund()
    {
        if (lastOpponentRound < manager.round && opponent_id != "")
        {
            string json_data = "{\"to\":\"" + opponent_id + "\"," +
                                "\"action\":\"" + local_action + "\"," +
                                "\"from_pos\":\"" + local_fromPos.ToString() + "\"," +
                                "\"to_pos\":\"" + local_toPos.ToString() + "\"," +
                                "\"nfs\":\"" + local_notFoundSwitched.ToString() + "\"," +
                                "\"round\":\"" + manager.round.ToString() + "\"}";
            socket.Emit("next_round", json_data);
        }
    }


    public void emitExit()
    {
        if (socket == null)
            return;

        socket.Emit("left", opponent_id);
    }


    string placement_list = "";
    public void emitCardPlacement(string list)
    {
        placement_list += list;
        if (socket == null || placement_list.Length<18)
            return;

        string json_data = "{\"placed_list\":\""+ placement_list + "\"," +
                           "\"to\":\""+opponent_id+"\"}";
        socket.Emit("update_place", json_data);
    }

    public void joinPublic(string nick, string v)
    {
        if (socket == null)
            return;

        string json_data = "{\"uid\":\"" + this.id + "\"," +
                           "\"v\":\"" + v + "\"," +
                           "\"nick\":\"" + nick + "\"}";

        Debug.Log("Join match");
        socket.Emit("join_public", json_data);
    }

    public void createPrivate(string nick, string v)
    {
        if (socket == null)
            return;

        string json_data = "{\"uid\":\"" + this.id + "\"," +
                           "\"v\":\"" + v + "\"," +
                           "\"nick\":\"" + nick + "\"}";

        socket.Emit("create_private", json_data);
    }

    public void joinPrivate(string code, string nick, string v)
    {
        if (socket == null)
            return;

        string json_data = "{\"uid\":\"" + this.id + "\"," +
                           "\"v\":\"" + v + "\"," + 
                           "\"nick\":\"" + nick + "\"," + 
                           "\"mid\":\"" + code + "\"}";
        socket.Emit("join_private", json_data);
    }

    public void emitMatchEnd(string status)
    {
        if (socket == null)
            return;
        string json_data = "{\"uid\":\"" + this.id + "\"," +
                           "\"match_id\":\"" + this.match_id + "\"," +
                           "\"status\":\"" + status + "\"}";
        socket.Emit("match_end", json_data);
    }

}
