namespace Assist.Objects.AssistApi.Valorant.Skin
{

    public class WeaponSkin
    {

        public string Uuid { get; set; }

        public string DisplayName { get; set; }

        public string ThemeUuid { get; set; }

        public string DisplayIcon { get; set; }

        public WeaponSkinChroma[] Chromas { get; set; }

        public WeaponSkinLevel[] Levels { get; set; }
    }
}
