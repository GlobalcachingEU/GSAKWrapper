using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSAKWrapper.UIControls.FormulaSolver
{
    public class StrRes
    {
        public static string STR_MISSING_ARGUMENT = "MissingArgument";
        public static string STR_TO_MUCH_ARGUMENTS = "TooManyArguments";
        public static string STR_VALUE_OUT_OF_RANGE = "ValueOutOfRange";
        public static string STR_BAD_PARAMETER_VALUE = "BadParameterValue";
        public static string STR_NO_CROSSING = "NoCrossing";
        public static string STR_MAX_1000_DIGITS = "PiMaxDigits";
        public static string STR_MIN_0_DIGITS = "PiMinDigits";
        public static string STR_UNKNOWN_FUNCTION = "UnknownFunction";
        public static string STR_MISSING_VARIABLES = "MissingVariables";
        public static string STR_INSERT_FORMULA = "InsertFormula";
        public static string STR_INSERT_WAYPOINT = "InsertWaypoint";
        public static string STR_SOLVE = "Solve";
        public static string STR_AS_WAYPOINT = "AsWaypoint";
        public static string STR_NUMBER_GROUP = "NumberFunctions";
        public static string STR_COORDINATE_GROUP = "CoordinateFunctions";
        public static string STR_TEXT_GROUP = "TextFunctions";
        public static string STR_UNKNOWN_GROUP = "UnknownFunctionGroup";
        public static string STR_DESCR_CROSSTOTAL = "DescrCrossTotal";
        public static string STR_DESCR_ICROSSTOTAL = "DescrICrossTotal";
        public static string STR_DESCR_CROSSPRODUCT = "DescrCrossProduct";
        public static string STR_DESCR_ICROSSPRODUCT = "DescrICrossProduct";
        public static string STR_DESCR_PRIMENUMBER = "DescrPrimeNumber";
        public static string STR_DESCR_PRIMEINDEX = "DescrPrimeIndex";
        public static string STR_DESCR_INT = "DescrInt";
        public static string STR_DESCR_ROUND = "DescrRound";
        public static string STR_DESCR_ROM2DEC = "DescrRom2Dec";
        public static string STR_DESCR_PI = "DescrPi";
        public static string STR_DESCR_BEARING = "DescrBearing";
        public static string STR_DESCR_DISTANCE = "DescrDistance";
        public static string STR_DESCR_CROSSBEARING = "DescrCrossBearing";
        public static string STR_DESCR_INTERSECTION = "DescrIntersection";
        public static string STR_DESCR_PROJECTION = "DescrProjection";
        public static string STR_DESCR_ALPHASUM = "DescrAlphaSum";
        public static string STR_DESCR_ALPHAPOS = "DescrAlphaPos";
        public static string STR_DESCR_PHONECODE = "DescrPhoneCode";
        public static string STR_DESCR_PHONESUM = "DescrPhoneSum";
        public static string STR_DESCR_LEN = "DescrLen";
        public static string STR_DESCR_MID = "DescrMid";
        public static string STR_DESCR_REVERSE = "DescrReverse";
        public static string STR_DESCR_ROT13 = "DescrRot13";
        public static string STR_DESCR_WAYPOINT = "DescrWaypoint";
        public static string STR_WPSEL_TITLE = "SelectWaypoint";
        public static string STR_WPSEL_WAYPOINTS = "Waypoints";
        public static string STR_WPSEL_INSERT = "Insert";
        public static string STR_WPSEL_CANCEL = "Cancel";
        public static string STR_INSFORM_TITLE = "InsertFormula";
        public static string STR_INSFORM_GROUP = "Group";
        public static string STR_INSFORM_FUNCTIONS = "Functions";
        public static string STR_INSFORM_OTHER = "OtherNames";
        public static string STR_INSFORM_DESCRIPTION = "Description";
        public static string STR_INSFORM_INSERT = "Insert";
        public static string STR_INSFORM_CANCEL = "Cancel";
        public static string STR_DIV_BY_ZERO = "DivByZero";
        public static string STR_DESCR_POW = "DescrPow";
        public static string STR_DESCR_FACTORIAL = "DescrFactorial";
        public static string STR_NO_CACHE_SELECTED = "DescrNoCacheSelected";
        public static string NO_PROPER_COORDINATES_SELECTED = "NoProperCoordSelected";
        public static string STR_DESCR_LATITUDE = "DescrLatitude";
        public static string STR_DESCR_LONGITUDE = "DescrLongitude";
        public static string STR_CONTEXT_GROUP = "ContextGroup";
        public static string STR_DESCR_CONTEXT = "DescrContext";
        public static string STR_TRIGONOMETRIC_GROUP = "TrigGroup";
        public static string STR_DESC_SIN = "DescrSin";
        public static string STR_DESC_COS = "DescrCos";
        public static string STR_DESC_TAN = "DescrTan";
        public static string STR_DESC_SINH = "DescrSinH";
        public static string STR_DESC_COSH = "DescrCosH";
        public static string STR_DESC_TANH = "DescrTanH";
        public static string STR_DESC_ASIN = "DescrASin";
        public static string STR_DESC_ACOS = "DescrACos";
        public static string STR_DESC_ATAN = "DescrATan";

        public static string GetString(string res)
        {
            return Localization.TranslationManager.Instance.Translate(res) as string;
        }
    }
}
