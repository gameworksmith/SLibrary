using UnityEngine;
using System;
using System.Collections.Generic;

class MobileConsoleGUI : MobileGUIBase {
    public static MobileConsoleGUI           Instance { get; private set; }
    public static bool EnableLog;
  
    private static readonly int             MAX_LOG = 200;
    private static readonly int             WND_ID = 0x1435;
    private static readonly float           EDGE_X = 16, EDGE_Y = 8;

    private readonly string[]               _logTypeNames;
    private readonly Queue<string>[]        _logInfoQueue;
    private readonly Vector2[]              _scrollPos;
    private int                             _logTypeChoose = (int)LogType.Log;
    private Rect                            _winRect;

    private MobileConsoleGUI () {
        _logTypeNames = Enum.GetNames ( typeof ( LogType ) );
        _logInfoQueue = new Queue<string>[ _logTypeNames.Length ];
        _scrollPos = new Vector2[ _logTypeNames.Length ];
        for ( int i = 0; i < _logInfoQueue.Length; ++i ) {
            _logInfoQueue[ i ] = new Queue<string> ( MAX_LOG );
            _scrollPos[ i ] = new Vector2 ( 0, 1 );
        }
    }

    void Start () {
        Instance = this;
        //BuglyAgent.RegisterLogCallback(LogCallback);
        //Application.logMessageReceived += LogCallback;
    }

    void OnEnable() {
        Application.logMessageReceivedThreaded += LogCallback;
    }

    void OnDisable() {
        Application.logMessageReceivedThreaded -= LogCallback;
    }

    void OnGUI () {
        if ( !Visible ) { return; }

        EventType et = Event.current.type;
        if ( et == EventType.Repaint || et == EventType.Layout ) {
            _winRect = new Rect(EDGE_X, EDGE_Y, Screen.width - EDGE_X * 2, Screen.height - EDGE_Y * 2 - BottomSpacing);
            GUI.skin.button.fontSize = GetSizeAdaptScreen(CustonFontSize);

            GUI.skin.verticalScrollbar.fixedWidth = Screen.width * 0.05f;
            GUI.skin.verticalScrollbarThumb.fixedWidth = Screen.width * 0.05f;

            GUI.Window ( WND_ID, _winRect, WindowFunc, string.Empty);
        }
    }

    void WindowFunc ( int id ) {
        try {
            GUILayout.BeginVertical ();
            try {
                _logTypeChoose = GUILayout.Toolbar(_logTypeChoose, _logTypeNames, GUILayout.Height(1.5f * GetSizeAdaptScreen(CustonFontSize)));
                var queue = _logInfoQueue[ _logTypeChoose ];
                if ( queue.Count > 0 ) {
                    _scrollPos[ _logTypeChoose ] = GUILayout.BeginScrollView ( _scrollPos[ _logTypeChoose ] );
                    try {
                        GUI.skin.label.fontSize = GetSizeAdaptScreen(CustonFontSize); 
                        foreach ( var s in queue ) {
                            GUILayout.Label ( s );
                        }
                    }
                    finally {
                        GUILayout.EndScrollView();
                    }
                }

              
            }
            finally {
                GUILayout.EndVertical ();
            }
        }
        catch ( Exception ex ) {
            Debug.LogException ( ex );
        }
    }

    static void Enqueue ( Queue<string> queue, string text, string stackTrace ) {
        while ( queue.Count >= MAX_LOG )
            queue.Dequeue ();
        queue.Enqueue ( text );
        if ( !string.IsNullOrEmpty ( stackTrace ) )
            queue.Enqueue ( stackTrace );
    }

    void LogCallback ( string condition, string stackTrace, LogType type ) {
        int index = ( int )type;
        var queue = _logInfoQueue[ index ];
        switch ( type ) {
            case LogType.Exception:
            case LogType.Error:
            case LogType.Warning:
                Enqueue ( queue, condition, stackTrace );
                break;
            default:
                Enqueue ( queue, condition, null );
                break;
        }
        _scrollPos[ index ] = new Vector2 ( 0, 100000f );
    }

    public void ClearLog() {
        foreach (var item in _logInfoQueue) {
            item.Clear();
        }
    }
}