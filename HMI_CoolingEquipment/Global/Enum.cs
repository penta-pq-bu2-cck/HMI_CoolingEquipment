namespace HMI_CoolingEquipment
{
    public enum CanonicalDataType
    {
        Unknown = 0,
        Int = 2,
        Long = 3,
        Real = 5,
        String = 8,
        Boolean = 11,
        Time = 19
    }

    public enum JobStatus
    {
        Initialize,
        Loading,
        Cooling,
        Staging,
        Complete,
        Error,
        LoadingTimeExpired,
        StagingTimeExpired,
        Purge,
    }

    public enum SettingsType
    {
        NumericUpDown,
        ToggleButton,
        ComboBox,
    }
    
    public enum LoadPortState
    {
        Disabled,
        Empty,
        LoadProcess,
        UnloadProcess,
        Loaded,
        CoolingCompleted,
        Error,
        Warning,
        SensorTesting,
        Abort,
    }

    public enum AlarmType
    {
        Error,
        Warning,
    }

    public enum HMIPage
    {
        MainPage,
        LoadingPage,
        UnloadingPage,
        SecsGemPage,
        PTLPage,
        Settings,
    }

    public enum HMIDBMemory
    {
        SETUPJOB,
        JOBDETAILS,
        USERACCOUNT,
        USERGROUPACCESS,
        LOADPORT,
        LOADPORTCONFIGURATION,
        ALARMCURRENT,
        ALARMHISTORY,
        SETTINGS,
        SETUPJOBHISTORY,
        ALL,
    }

    public enum LEDColour
    {
        RED,
        GREEN,
        AMBER,
        BLUE,
        PINK,
        CYAN,
        NoColour,
    }

    public enum LEDState
    {
        NoColour,
        NoBlinking,
        Blinking = 3,
    }

    public enum HMIReset
    {
        Reset,
        ResetJobHistory,
    }

    public enum LoadingModeKey
    {
        CarrierID,
        LotID,
    }
}
