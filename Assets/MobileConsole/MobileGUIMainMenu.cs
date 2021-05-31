using System;
using UnityEngine;
using System.Collections.Generic;

public class MobileGUIMainMenu : MobileGUIBase {
    private const float                     TOUCH_TIME = 5f;
    private static readonly int             MAX_LOG = 100;
    private static readonly int             WND_ID = 0x1435;
    private static readonly float           PADDING_X = 16, PADDING_Y = 8;

    private float                           _startTimer = 0;
    private float                           _coolDown = 0;
    private Rect                            _winRect;

    private bool                            _showChildPanel;
    private MobileGUIBase                   _currentChildGUI;

    private float                           _menuBtnHeight;
    private float                           _menuBtnSpacing;

    private string _validPwd = "99999999";

    [SerializeField] 
    private string _currentInputPassword;

    private static List<List<int>> areaCode = new List<List<int>>(){
		new List<int>{7,4,1},
		new List<int>{8,5,2},
		new List<int>{9,6,3},
	};


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        _menuBtnHeight = GetSizeAdaptScreen(60);
        _menuBtnSpacing = GetSizeAdaptScreen(20); 
    }

    private void Update() {
//#if ( UNITY_EDITOR || UNITY_WEBPLAYER )
//        if (Input.GetMouseButton(0) && Input.GetMouseButton(1) && Time.time - _coolDown > TOUCH_TIME) {
//#else
//        if( Input.touches.Length == 3 && Time.time - _coolDown > TOUCH_TIME) {
//#endif
//            _startTimer += InternalTime.deltaTime;
//            if (_startTimer > TOUCH_TIME) {
//                Visible = !Visible;
//                _coolDown = Time.time;
//                _startTimer = 0;
//            }
//        }
//        else {
//            _startTimer = 0;
//        }

        if (CheckInputSucceed()) {
            Visible = true;
            // RootManager.instance.EnableSuperBlock(true);
        }
    }

    private void OnGUI() {
        if (!Visible) { return; }

        GUI.skin.button.fontSize = GetSizeAdaptScreen(CustonFontSize);
        if (_showChildPanel) {
            var backRect = new Rect(_winRect.xMin, Screen.height - BottomSpacing, _winRect.width, BottomSpacing - PADDING_Y);

            if (GUI.Button(backRect, "Back To Main Menu")) {
                _showChildPanel = false;

                if (_currentChildGUI) {
                    _currentChildGUI.Visible = false;
                }
            }
        }
        else {
            _winRect = new Rect(PADDING_X, PADDING_Y, Screen.width - PADDING_X * 2, Screen.height - PADDING_Y * 2);
            GUI.Window(WND_ID, _winRect, DisplayWindowFunction, string.Empty);
        }
    }


    private static int CalcAreaCodeIn9RectangeGrid(float xLen, float yLen, Vector2 pos) {
        /*
        -------------
        - 1 - 2 - 3 -			  	
        -------------	 
        - 4 - 5 - 6 -  
        -------------
        - 7 - 8 - 9 -	
        -------------
        */

        int xIndex = CalcEqualIndex(xLen, pos.x);
        int yIndex = CalcEqualIndex(yLen, pos.y); ;

        int code = areaCode[xIndex][yIndex];
        return code;
    }

    private static int CalcEqualIndex(float len, float value) {
        //Debug.Assert(len >= value, "CalcEqualIndex, value must be smaller than len!");
        value = value > len ? len : value;
        value = value < 0 ? 0 : value;

        float threeEqualLen = len / 3f;
        if (value < threeEqualLen) {
            return 0;
        }
        else if (value >= threeEqualLen && value < threeEqualLen * 2) {
            return 1;
        }
        else {
            return 2;
        }
    }

    private bool CheckInputSucceed() {
        if (Visible) {
            return false;
        }
        if (Input.GetMouseButtonUp(0)) {
            int screenCode = CalcAreaCodeIn9RectangeGrid(Screen.width, Screen.height, Input.mousePosition);

            _currentInputPassword += screenCode.ToString();
            string subPwd = _validPwd.Substring(0, _currentInputPassword.Length);
            if (_currentInputPassword != subPwd) {
                //Debug.Log("pwd has been not corrected.reset!");
                _currentInputPassword = string.Empty;
            }
            else {
                //Debuger.LogFormat("当前密码输入：{0}", _currentInputPassword);
                if (_currentInputPassword == _validPwd) {
                    Debug.Log("mobile console gui password input valid.");
                    _currentInputPassword = string.Empty;
                    return true;
                }
            }
        }
        return false;
    }

    private void DisplayWindowFunction(int id) {
        if (GUI.Button(new Rect(PADDING_X, _menuBtnSpacing, _winRect.width - PADDING_X * 2, _menuBtnHeight), 
			"Log Information Entrance")) {
            _currentChildGUI = MobileConsoleGUI.Instance;
            _showChildPanel = true;
            _currentChildGUI.Visible = true;
        }
       
        // var closeBtnRect = new Rect(_winRect.xMin, Screen.height - BottomSpacing, _winRect.width - PADDING_X * 2, _menuBtnHeight);
        // if (GUI.Button(closeBtnRect, "<<<Close>>>")) {
        //     _currentChildGUI = null;
        //     Visible = false;
        // }

        // if (GUI.Button(new Rect(PADDING_X, _menuBtnHeight * 1 + _menuBtnSpacing * 2, _winRect.width - PADDING_X * 2, _menuBtnHeight),
        //     "修改日志等级")) {
        //     _currentChildGUI = MobileLogLevelGUI.Instance;
        //     _showChildPanel = true;
        //     _currentChildGUI.Visible = true;
        // }
        //
        //
        // if (GUI.Button(new Rect(PADDING_X, _menuBtnHeight * 2 + _menuBtnSpacing * 3, _winRect.width - PADDING_X * 2, _menuBtnHeight),
        //     "Open GM Tool")) {
        //     _currentChildGUI = MobileGmToolGUI.Instance;
        //     _showChildPanel = true;
        //     _currentChildGUI.Visible = true;
        // }

        // if (GUI.Button(new Rect(PADDING_X, _menuBtnHeight * 3 + _menuBtnSpacing * 4, _winRect.width - PADDING_X * 2, _menuBtnHeight),
        //     "Open/Close FPS")) {
        //     var fpsMonitor = GameObject.Find("GameLaucher").GetComponentInChildren<FPSMonitor>();
        //     fpsMonitor.Visible = !fpsMonitor.Visible;
        // }

        
        // if (GUI.Button(new Rect(PADDING_X, _menuBtnHeight * 4 + _menuBtnSpacing * 5, _winRect.width - PADDING_X * 2, _menuBtnHeight),
        //     DebugSettings.GuideEnableString)) {
        //     DebugSettings.DisableGuide = !DebugSettings.DisableGuide;
        // }

        // if (GUI.Button(new Rect(PADDING_X, _menuBtnHeight * 5 + _menuBtnSpacing * 6, _winRect.width - PADDING_X * 2, _menuBtnHeight),
        //     "执行lua代码")) {
        //     _currentChildGUI = MobileLuaToolGUI.Instance;
        //     _showChildPanel = true;
        //     _currentChildGUI.Visible = true;
        // }
        //
        // if (GUI.Button(new Rect(PADDING_X, _menuBtnHeight * 6 + _menuBtnSpacing * 7, _winRect.width - PADDING_X * 2, _menuBtnHeight),
        //     DebugSettings.EnableLogString)) {
        //     DebugSettings.EnableLog = !DebugSettings.EnableLog;
        // }

    }

}
