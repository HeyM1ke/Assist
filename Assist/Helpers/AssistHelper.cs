using System.Collections.Generic;
using AssistUser.Lib.V2.Models.Dodge;

namespace Assist.Core.Helpers;

public class AssistHelper
{
     public static Dictionary<EAssistDodgeCategory, string> DodgeCategories = new Dictionary<EAssistDodgeCategory, string>
    {
        { EAssistDodgeCategory.TOXIC, Properties.Resources.DodgeCategory_Toxic },
        { EAssistDodgeCategory.THROWER, Properties.Resources.DodgeCategory_Thrower },
        { EAssistDodgeCategory.BADTEAMMATE, Properties.Resources.DodgeCategory_BadTeammate },
        { EAssistDodgeCategory.STREAMSNIPER, Properties.Resources.DodgeCategory_Streamsniper },
        { EAssistDodgeCategory.IGNORE, Properties.Resources.DodgeCategory_Ignore },
        { EAssistDodgeCategory.WINTRADER, Properties.Resources.DodgeCategory_Wintrader },
        { EAssistDodgeCategory.CHEATER, Properties.Resources.DodgeCategory_Cheater },
        { EAssistDodgeCategory.BADCOMMS, Properties.Resources.DodgeCategory_BadComms},
        { EAssistDodgeCategory.GRIEFER, Properties.Resources.DodgeCategory_Griefer },
        { EAssistDodgeCategory.BOT, Properties.Resources.DodgeCategory_Bot },
        { EAssistDodgeCategory.SMURF, Properties.Resources.DodgeCategory_Smurf },
        
    };
}