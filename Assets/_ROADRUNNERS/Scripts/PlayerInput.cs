using System;
using UnityEngine;


internal enum InputState { Default, Pressed, Sustained, Released }

public class PlayerInput : MonoBehaviour
{
    internal enum ControllerID { Controller0, Controller1, Controller2, Controller3, Controller4}
    [SerializeField] internal ControllerID _controllerID = ControllerID.Controller0;
    ControllerID _controllerLast;

    //internal enum InputState { Default, Pressed, Sustained, Released }

    [HideInInspector]
    internal bool
        StartPressed, StartSustained, StartReleased,
        SelectPressed, SelectSustained, SelectReleased,

        LBumperPressed, LBumperSustained, LBumperReleased,
        RBumperPressed, RBumperSustained, RBumperReleased,

        AButtonPressed, AButtonSustained, AButtonReleased,
        BButtonPressed, BButtonSustained, BButtonReleased,
        XButtonPressed, XButtonSustained, XButtonReleased,
        YButtonPressed, YButtonSustained, YButtonReleased,

        LSButtonPressed, LSButtonSustained, LSButtonReleased,
        RSButtonPressed, RSButtonSustained, RSButtonReleased,

        LTriggerPressed, LTriggerSustained, LTriggerReleased,
        RTriggerPressed, RTriggerSustained, RTriggerReleased,

        NPaddlePressed, NPaddleSustained, NPaddleReleased,
        EPaddlePressed, EPaddleSustained, EPaddleReleased,
        SPaddlePressed, SPaddleSustained, SPaddleReleased,
        WPaddlePressed, WPaddleSustained, WPaddleReleased;

    #region Inputs

    string
        StartButtonInput, SelectButtonInput,

        AButtonInput, BButtonInput,
        XButtonInput, YButtonInput,

        LBumperInput, RBumperInput,

        LStickInput, LStickInputX, LStickInputY,
        RStickInput, RStickInputX, RStickInputY,

        LTriggerInput, RTriggerInput,

        DPadInputX, DPadInputY;

    #endregion

    #region Buttons

    [SerializeField]
    InputState
        LBumperState = InputState.Default,
        RBumperState = InputState.Default,

        AButtonState = InputState.Default,
        BButtonState = InputState.Default,
        XButtonState = InputState.Default,
        YButtonState = InputState.Default,

        StartButtonState = InputState.Default,
        SelectButtonState = InputState.Default;

    #endregion

    #region Joysticks

    [SerializeField]
    InputState
        LSButtonState = InputState.Default,
        RSButtonState = InputState.Default;

    internal float
        LStickAxisX, LStickAxisY,
        RStickAxisX, RStickAxisY;

    [SerializeField]
    internal Vector2
        LStickVector,
        RStickVector;

    #endregion

    #region Triggers

    [SerializeField]
    InputState
        LTriggerState = InputState.Default,
        RTriggerState = InputState.Default;

    [SerializeField]
    [Range(0f, 1f)]
    internal float
        LTriggerAxisZ,
        RTriggerAxisZ;

    private float
        LTriggerLast = 0f,
        RTriggerLast = 0f;

    #endregion

    #region Paddle

    [SerializeField]
    InputState
        NPaddleState = InputState.Default,
        EPaddleState = InputState.Default,
        SPaddleState = InputState.Default,
        WPaddleState = InputState.Default;

    internal float
        DPadAxisX, DPadAxisY;

    private float
        DPadLastX = 0f,
        DPadLastY = 0f;

    [SerializeField] internal Vector2 DPadVector;

    #endregion

    private void Start()
        => UpdateInputPaths();

    void Update()
    {
        #region Bumpers

        if (Input.GetButtonDown(LBumperInput))
            LBumperState = InputState.Pressed;

        else if (Input.GetButton(LBumperInput))
            LBumperState = InputState.Sustained;

        else if (Input.GetButtonUp(LBumperInput))
            LBumperState = InputState.Released;

        else LBumperState = InputState.Default;

        LBumperPressed   = LBumperState == InputState.Pressed;
        LBumperSustained = LBumperState == InputState.Sustained;
        LBumperReleased  = LBumperState == InputState.Released;


        if (Input.GetButtonDown(RBumperInput))
            RBumperState = InputState.Pressed;

        else if (Input.GetButton(RBumperInput))
            RBumperState = InputState.Sustained;

        else if (Input.GetButtonUp(RBumperInput))
            RBumperState = InputState.Released;

        else RBumperState = InputState.Default;

        RBumperPressed   = RBumperState == InputState.Pressed;
        RBumperSustained = RBumperState == InputState.Sustained;
        RBumperReleased  = RBumperState == InputState.Released;

        #endregion

        #region Joysticks

        if (Input.GetButtonDown(LStickInput))
            LSButtonState = InputState.Pressed;

        else if (Input.GetButton(LStickInput))
            LSButtonState = InputState.Sustained;

        else if (Input.GetButtonUp(LStickInput))
            LSButtonState = InputState.Released;

        else LSButtonState = InputState.Default;

        LSButtonPressed   = LSButtonState == InputState.Pressed;
        LSButtonSustained = LSButtonState == InputState.Sustained;
        LSButtonReleased  = LSButtonState == InputState.Released;

        if (Input.GetButtonDown(RStickInput))
            RSButtonState = InputState.Pressed;

        else if (Input.GetButton(RStickInput))
            RSButtonState = InputState.Sustained;

        else if (Input.GetButtonUp(RStickInput))
            RSButtonState = InputState.Released;

        else RSButtonState = InputState.Default;

        RSButtonPressed   = RSButtonState == InputState.Pressed;
        RSButtonSustained = RSButtonState == InputState.Sustained;
        RSButtonReleased  = RSButtonState == InputState.Released;

        LStickAxisX = Input.GetAxis(LStickInputX);
        LStickAxisY = Input.GetAxis(LStickInputY);
        LStickVector = new Vector2(LStickAxisX, LStickAxisY);

        RStickAxisX = Input.GetAxis(RStickInputX);
        RStickAxisY = Input.GetAxis(RStickInputY);
        RStickVector = new Vector2(RStickAxisX, RStickAxisY);

        #endregion

        #region Triggers

        LTriggerAxisZ = Input.GetAxis(LTriggerInput);

        if (LTriggerLast == 0 && LTriggerAxisZ != 0)
            LTriggerState = InputState.Pressed;

        else if (LTriggerLast != 0 && LTriggerAxisZ != 0)
            LTriggerState = InputState.Sustained;

        else if (LTriggerLast != 0 && LTriggerAxisZ == 0)
            LTriggerState = InputState.Released;

        else LTriggerState = InputState.Default;

        LTriggerPressed   = LTriggerState == InputState.Pressed;
        LTriggerSustained = LTriggerState == InputState.Sustained;
        LTriggerReleased  = LTriggerState == InputState.Released;


        RTriggerAxisZ = Input.GetAxis(RTriggerInput);

        if (RTriggerLast == 0 && RTriggerAxisZ != 0)
            RTriggerState = InputState.Pressed;

        else if (RTriggerLast != 0 && RTriggerAxisZ != 0)
            RTriggerState = InputState.Sustained;

        else if (RTriggerLast != 0 && RTriggerAxisZ == 0)
            RTriggerState = InputState.Released;

        else RTriggerState = InputState.Default;

        RTriggerPressed   = RTriggerState == InputState.Pressed;
        RTriggerSustained = RTriggerState == InputState.Sustained;
        RTriggerReleased  = RTriggerState == InputState.Released;

        #endregion

        #region Buttons

        if (Input.GetButtonDown(StartButtonInput))
            StartButtonState = InputState.Pressed;

        else if (Input.GetButton(StartButtonInput))
            StartButtonState = InputState.Sustained;

        else if (Input.GetButtonUp(StartButtonInput))
            StartButtonState = InputState.Released;

        else StartButtonState = InputState.Default;

        StartPressed   = StartButtonState == InputState.Pressed;
        StartSustained = StartButtonState == InputState.Sustained;
        StartReleased  = StartButtonState == InputState.Released;


        if (Input.GetButtonDown(SelectButtonInput))
            SelectButtonState = InputState.Pressed;

        else if (Input.GetButton(SelectButtonInput))
            SelectButtonState = InputState.Sustained;

        else if (Input.GetButtonUp(SelectButtonInput))
            SelectButtonState = InputState.Released;

        else SelectButtonState = InputState.Default;

        SelectPressed   = SelectButtonState == InputState.Pressed;
        SelectSustained = SelectButtonState == InputState.Sustained;
        SelectReleased  = SelectButtonState == InputState.Released;


        if (Input.GetButtonDown(AButtonInput))
            AButtonState = InputState.Pressed;

        else if (Input.GetButton(AButtonInput))
            AButtonState = InputState.Sustained;

        else if (Input.GetButtonUp(AButtonInput))
            AButtonState = InputState.Released;

        else AButtonState = InputState.Default;

        AButtonPressed   = AButtonState == InputState.Pressed;
        AButtonSustained = AButtonState == InputState.Sustained;
        AButtonReleased  = AButtonState == InputState.Released;


        if (Input.GetButtonDown(BButtonInput))
            BButtonState = InputState.Pressed;

        else if (Input.GetButton(BButtonInput))
            BButtonState = InputState.Sustained;

        else if (Input.GetButtonUp(BButtonInput))
            BButtonState = InputState.Released;

        else BButtonState = InputState.Default;

        BButtonPressed   = BButtonState == InputState.Pressed;
        BButtonSustained = BButtonState == InputState.Sustained;
        BButtonReleased  = BButtonState == InputState.Released;


        if (Input.GetButtonDown(XButtonInput))
            XButtonState = InputState.Pressed;

        else if (Input.GetButton(XButtonInput))
            XButtonState = InputState.Sustained;

        else if (Input.GetButtonUp(XButtonInput))
            XButtonState = InputState.Released;

        else XButtonState = InputState.Default;


        XButtonPressed   = XButtonState == InputState.Pressed;
        XButtonSustained = XButtonState == InputState.Sustained;
        XButtonReleased  = XButtonState == InputState.Released;

        if (Input.GetButtonDown(YButtonInput))
            YButtonState = InputState.Pressed;

        else if (Input.GetButton(YButtonInput))
            YButtonState = InputState.Sustained;

        else if (Input.GetButtonUp(YButtonInput))
            YButtonState = InputState.Released;

        else YButtonState = InputState.Default;

        YButtonPressed   = YButtonState == InputState.Pressed;
        YButtonSustained = YButtonState == InputState.Sustained;
        YButtonReleased  = YButtonState == InputState.Released;

        #endregion

        #region Paddle

        DPadAxisX = Input.GetAxis(DPadInputX);
        DPadAxisY = Input.GetAxis(DPadInputY);

        DPadVector = new Vector2(DPadAxisX, DPadAxisY);


        if (DPadAxisY > 0 && DPadAxisY > DPadLastY)
            NPaddleState = InputState.Pressed;

        else if (DPadAxisY > 0 && DPadAxisY == DPadLastY)
            NPaddleState = InputState.Sustained;

        else if (DPadAxisY == 0 && DPadAxisY < DPadLastY)
            NPaddleState = InputState.Released;

        else NPaddleState = InputState.Default;

        NPaddlePressed = NPaddleState == InputState.Pressed;
        NPaddleSustained = NPaddleState == InputState.Sustained;
        NPaddleReleased = NPaddleState == InputState.Released;


        if (DPadAxisX > 0 && DPadAxisX > DPadLastX)
            EPaddleState = InputState.Pressed;

        else if (DPadAxisX > 0 && DPadAxisX == DPadLastX)
            EPaddleState = InputState.Sustained;

        else if (DPadAxisX == 0 && DPadAxisX < DPadLastX)
            EPaddleState = InputState.Released;

        else EPaddleState = InputState.Default;

        EPaddlePressed = EPaddleState == InputState.Pressed;
        EPaddleSustained = EPaddleState == InputState.Sustained;
        EPaddleReleased = EPaddleState == InputState.Released;


        if (DPadAxisY < 0 && DPadAxisY < DPadLastY)
            SPaddleState = InputState.Pressed;

        else if (DPadAxisY < 0 && DPadAxisY == DPadLastY)
            SPaddleState = InputState.Sustained;

        else if (DPadAxisY == 0 && DPadAxisY > DPadLastY)
            SPaddleState = InputState.Released;

        else SPaddleState = InputState.Default;

        SPaddlePressed = SPaddleState == InputState.Pressed;
        SPaddleSustained = SPaddleState == InputState.Sustained;
        SPaddleReleased = SPaddleState == InputState.Released;


        if (DPadAxisX < 0 && DPadAxisX < DPadLastX)
            WPaddleState = InputState.Pressed;

        else if (DPadAxisX < 0 && DPadAxisX == DPadLastX)
            WPaddleState = InputState.Sustained;

        else if (DPadAxisX == 0 && DPadAxisX > DPadLastX)
            WPaddleState = InputState.Released;

        else WPaddleState = InputState.Default;

        WPaddlePressed = WPaddleState == InputState.Pressed;
        WPaddleSustained = WPaddleState == InputState.Sustained;
        WPaddleReleased = WPaddleState == InputState.Released;

        #endregion
    }

    private void LateUpdate()
    {
        if (_controllerID != _controllerLast)
            UpdateInputPaths();

        _controllerLast = _controllerID;

        LTriggerLast = LTriggerAxisZ;
        RTriggerLast = RTriggerAxisZ;

        DPadLastX = DPadAxisX;
        DPadLastY = DPadAxisY;

    }

    private void UpdateInputPaths()
    {
        StartButtonInput = $"Gamepad({(int)_controllerID}).Start";
        SelectButtonInput = $"Gamepad({(int)_controllerID}).Select";

        AButtonInput = $"Gamepad({(int)_controllerID}).AButton";
        BButtonInput = $"Gamepad({(int)_controllerID}).BButton";
        XButtonInput = $"Gamepad({(int)_controllerID}).XButton";
        YButtonInput = $"Gamepad({(int)_controllerID}).YButton";

        LBumperInput = $"Gamepad({(int)_controllerID}).LBumper";
        RBumperInput = $"Gamepad({(int)_controllerID}).RBumper";

        LStickInput  = $"Gamepad({(int)_controllerID}).LStick";
        LStickInputX = $"Gamepad({(int)_controllerID}).LStick.x";
        LStickInputY = $"Gamepad({(int)_controllerID}).LStick.y";
        RStickInput  = $"Gamepad({(int)_controllerID}).RStick";
        RStickInputX = $"Gamepad({(int)_controllerID}).RStick.x";
        RStickInputY = $"Gamepad({(int)_controllerID}).RStick.y";

        LTriggerInput = $"Gamepad({(int)_controllerID}).LTrigger.z";
        RTriggerInput = $"Gamepad({(int)_controllerID}).RTrigger.z";

        DPadInputX = $"Gamepad({(int)_controllerID}).DPad.x";
        DPadInputY = $"Gamepad({(int)_controllerID}).DPad.y";
    }
}