using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Models.Socket;
using ValNet.Objects.Local;

namespace Assist.Core.Helpers;

public class ValorantHelper
{
     public static Dictionary<string, string> Servers = new Dictionary<string, string>
    {
        { "aresqa.aws-rclusterprod-use1-1.dev1-gp-ashburn-1", "Ashburn" },
        { "aresqa.aws-use1-dev.main1-gp-ashburn-1", "Ashburn" },
        { "aresriot.aws-mes1-prod.eu-gp-bahrain-1", "Bahrain" },
        { "aresriot.aws-mes1-prod.ext1-gp-bahrain-1", "Bahrain" },
        { "aresriot.aws-mes1-prod.tournament-gp-bahrain-1", "Bahrain" },
        { "aresriot.aws-rclusterprod-mes1-1.eu-gp-bahrain-awsedge-1", "Bahrain" },
        { "aresriot.aws-rclusterprod-mes1-1.ext1-gp-bahrain-awsedge-1", "Bahrain" },
        { "aresriot.aws-rclusterprod-mes1-1.tournament-gp-bahrain-awsedge-1", "Bahrain" },
        { "loltencent.qcloud.val-gp-beijing-1", "Beijing" },
        { "aresriot.aws-rclusterprod-bog1-1.latam-gp-bogota-1", "Bogotá" },
        { "aresriot.aws-rclusterprod-bog1-1.tournament-gp-bogota-1", "Bogotá" },
        { "aresriot.aws-rclusterprod-usw2-1.tournament-gp-cmob-1", "CMOB 1" },
        { "aresriot.aws-rclusterprod-usw2-1.tournament-gp-cmob-2", "CMOB 2" },
        { "aresriot.aws-rclusterprod-usw2-1.tournament-gp-cmob-3", "CMOB 3" },
        { "aresriot.aws-rclusterprod-usw2-1.tournament-gp-cmob-4", "CMOB 4" },
        { "aresriot.aws-chi1-prod.ext1-gp-chicago-1", "Chicago" },
        { "aresriot.aws-chi1-prod.latam-gp-chicago-1", "Chicago" },
        { "aresriot.aws-ord1-prod.ext1-gp-chicago-1", "Chicago" },
        { "aresriot.aws-ord1-prod.latam-gp-chicago-1", "Chicago" },
        { "aresriot.mtl-riot-ord2-3.ext1-gp-chicago-1", "Chicago" },
        { "aresriot.mtl-riot-ord2-3.latam-gp-chicago-1", "Chicago" },
        { "loltencent.qcloud.val-gp-chongqing-1", "Chongqing" },
        { "arestencent.qcloud-cq1.alpha1-gp-1", "Chongqing 1" },
        { "arestencent.qcloud-cq1.alpha1-gp-2", "Chongqing 2" },
        { "aresqa.aws-rclusterprod-dfw1-1.dev1-gp-dallas-1", "Dallas" },
        { "aresqa.aws-euc1-dev.main1-gp-frankfurt-1", "Frankfurt" },
        { "aresqa.aws-euc1-dev.stage1-gp-frankfurt-1", "Frankfurt" },
        { "aresqa.aws-rclusterprod-euc1-1.dev1-gp-frankfurt-1", "Frankfurt" },
        { "aresqa.aws-rclusterprod-euc1-1.stage1-gp-frankfurt-1", "Frankfurt" },
        { "aresriot.aws-euc1-prod.eu-gp-frankfurt-1", "Frankfurt" },
        { "aresriot.aws-euc1-prod.ext1-gp-eu1", "Frankfurt" },
        { "aresriot.aws-euc1-prod.ext1-gp-frankfurt-1", "Frankfurt" },
        { "aresriot.aws-euc1-prod.tournament-gp-frankfurt-1", "Frankfurt" },
        { "aresriot.aws-rclusterprod-euc1-1.ext1-gp-eu1", "Frankfurt" },
        { "aresriot.aws-rclusterprod-euc1-1.tournament-gp-frankfurt-1", "Frankfurt" },
        { "aresriot.aws-rclusterprod-euc1-1.eu-gp-frankfurt-1", "Frankfurt 1" },
        { "aresriot.aws-rclusterprod-euc1-1.eu-gp-frankfurt-awsedge-1", "Frankfurt 2" },
        { "aresqa.aws-atl1-dev.main1-gp-atlanta-1", "Georgia" },
        { "loltencent.qcloud.val-gp-guangzhou-1", "Guangzhou" },
        { "arestencent.qcloud-gz1.alpha1-gp-1", "Guangzhou 1" },
        { "arestencent.qcloud-gz1.alpha1-gp-2", "Guangzhou 2" },
        { "aresriot.aws-ape1-prod.ap-gp-hongkong-1", "Hong Kong" },
        { "aresriot.aws-ape1-prod.ext1-gp-hongkong-1", "Hong Kong" },
        { "aresriot.aws-ape1-prod.tournament-gp-hongkong-1", "Hong Kong" },
        { "aresriot.aws-rclusterprod-ape1-1.ext1-gp-hongkong-1", "Hong Kong" },
        { "aresriot.aws-rclusterprod-ape1-1.tournament-gp-hongkong-1", "Hong Kong" },
        { "aresriot.aws-rclusterprod-ape1-1.ap-gp-hongkong-1", "Hong Kong 1" },
        { "aresriot.aws-rclusterprod-ape1-1.ap-gp-hongkong-awsedge-1", "Hong Kong 2" },
        { "aresriot.mtl-riot-ist1-2.eu-gp-istanbul-1", "Istanbul" },
        { "aresriot.mtl-riot-ist1-2.tournament-gp-istanbul-1", "Istanbul" },
        { "aresriot.aws-euw2-prod.eu-gp-london-1", "London" },
        { "aresriot.aws-euw2-prod.tournament-gp-london-1", "London" },
        { "aresriot.aws-rclusterprod-euw2-1.eu-gp-london-awsedge-1", "London" },
        { "aresriot.aws-rclusterprod-euw2-1.tournament-gp-london-awsedge-1", "London" },
        { "aresriot.aws-rclusterprod-mad1-1.eu-gp-madrid-1", "Madrid" },
        { "aresriot.aws-rclusterprod-mad1-1.tournament-gp-madrid-1", "Madrid" },
        { "aresriot.aws-eus2-prod.eu-gp-madrid-1", "Madrid" },
        { "aresriot.aws-eus2-prod.eu-gp-madrid-2", "Madrid" },
        { "aresriot.mtl-tmx-mex1-1.ext1-gp-mexicocity-1", "Mexico City" },
        { "aresriot.mtl-tmx-mex1-1.latam-gp-mexicocity-1", "Mexico City" },
        { "aresriot.mtl-tmx-mex1-1.tournament-gp-mexicocity-1", "Mexico City" },
        { "aresriot.aws-mia1-prod.latam-gp-miami-1", "Miami" },
        { "aresriot.aws-mia1-prod.tournament-gp-miami-1", "Miami" },
        { "aresriot.mia1.latam-gp-miami-1", "Miami" },
        { "aresriot.mia1.tournament-gp-miami-1", "Miami" },
        { "aresriot.aws-aps1-prod.ap-gp-mumbai-1", "Mumbai" },
        { "aresriot.aws-aps1-prod.tournament-gp-mumbai-1", "Mumbai" },
        { "aresriot.aws-rclusterprod-aps1-1.ap-gp-mumbai-awsedge-1", "Mumbai" },
        { "aresriot.aws-rclusterprod-aps1-1.tournament-gp-mumbai-awsedge-1", "Mumbai" },
        { "aresqa.aws-rclusterprod-usw1-1.dev1-gp-1", "N. California" },
        { "aresqa.aws-usw1-dev.main1-gp-norcal-1", "N. California" },
        { "arestencentqa.qcloud-nj1.stage1-gp-1", "Nanjing" },
        { "arestencent.qcloud-nj1.alpha1-gp-1", "Nanjing 1" },
        { "arestencent.qcloud-nj1.alpha1-gp-2", "Nanjing 2" },
        { "aresriot.aws-rclusterprod-usw2-1.tournament-gp-offline-1", "Offline 1" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-1", "Offline 1" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-10", "Offline 10" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-11", "Offline 11" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-12", "Offline 12" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-13", "Offline 13" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-14", "Offline 14" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-15", "Offline 15" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-16", "Offline 16" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-17", "Offline 17" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-18", "Offline 18" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-19", "Offline 19" },
        { "aresriot.aws-rclusterprod-usw2-1.tournament-gp-offline-2", "Offline 2" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-2", "Offline 2" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-20", "Offline 20" },
        { "aresriot.aws-rclusterprod-usw2-1.tournament-gp-offline-3", "Offline 3" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-3", "Offline 3" },
        { "aresriot.aws-rclusterprod-usw2-1.tournament-gp-offline-4", "Offline 4" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-4", "Offline 4" },
        { "aresriot.aws-rclusterprod-usw2-1.tournament-gp-offline-5", "Offline 5" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-5", "Offline 5" },
        { "aresriot.aws-rclusterprod-usw2-1.tournament-gp-offline-6", "Offline 6" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-6", "Offline 6" },
        { "aresriot.aws-rclusterprod-usw2-1.tournament-gp-offline-7", "Offline 7" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-7", "Offline 7" },
        { "aresriot.aws-rclusterprod-usw2-1.tournament-gp-offline-8", "Offline 8" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-8", "Offline 8" },
        { "aresriot.aws-usw2-prod.tournament-gp-offline-9", "Offline 9" },
        { "aresriot.aws-euw3-prod.eu-gp-paris-1", "Paris" },
        { "aresriot.aws-euw3-prod.tournament-gp-paris-1", "Paris" },
        { "aresriot.aws-rclusterprod-euw3-1.tournament-gp-paris-1", "Paris" },
        { "aresriot.aws-rclusterprod-euw3-1.eu-gp-paris-1", "Paris 1" },
        { "aresriot.aws-rclusterprod-euw3-1.eu-gp-paris-awsedge-1", "Paris 2" },
        { "globaltencent.tcc-tcloudtest-sjc1-1.val-gp-1", "SJC" },
        { "aresriot.mtl-ctl-scl2-2.ext1-gp-santiago-1", "Santiago" },
        { "aresriot.mtl-ctl-scl2-2.latam-gp-santiago-1", "Santiago" },
        { "aresriot.mtl-ctl-scl2-2.tournament-gp-santiago-1", "Santiago" },
        { "aresriot.aws-rclusterprod-sae1-1.ext1-gp-saopaulo-1", "Sao Paulo" },
        { "aresriot.aws-rclusterprod-sae1-1.tournament-gp-saopaulo-1", "Sao Paulo" },
        { "aresriot.aws-sae1-prod.br-gp-saopaulo-1", "Sao Paulo" },
        { "aresriot.aws-sae1-prod.ext1-gp-saopaulo-1", "Sao Paulo" },
        { "aresriot.aws-sae1-prod.tournament-gp-saopaulo-1", "Sao Paulo" },
        { "aresriot.aws-rclusterprod-sae1-1.br-gp-saopaulo-1", "Sao Paulo 1" },
        { "aresriot.aws-rclusterprod-sae1-1.br-gp-saopaulo-awsedge-1", "Sao Paulo 2" },
        { "aresriot.aws-apne2-prod.ext1-gp-seoul-1", "Seoul" },
        { "aresriot.aws-apne2-prod.kr-gp-seoul-1", "Seoul" },
        { "aresriot.aws-apne2-prod.tournament-gp-seoul-1", "Seoul" },
        { "aresriot.aws-rclusterprod-apne2-1.ext1-gp-seoul-1", "Seoul" },
        { "aresriot.aws-rclusterprod-apne2-1.tournament-gp-seoul-1", "Seoul" },
        { "aresriot.aws-rclusterprod-apne2-1.kr-gp-seoul-1", "Seoul 1" },
        { "loltencent.qcloud.val-gp-shanghai-1", "Shanghai" },
        { "aresqa.aws-apse1-dev.main1-gp-singapore-1", "Singapore" },
        { "aresriot.aws-apse1-prod.ap-gp-singapore-1", "Singapore" },
        { "aresriot.aws-apse1-prod.ext1-gp-singapore-1", "Singapore" },
        { "aresriot.aws-apse1-prod.tournament-gp-singapore-1", "Singapore" },
        { "aresriot.aws-rclusterprod-apse1-1.ext1-gp-singapore-1", "Singapore" },
        { "aresriot.aws-rclusterprod-apse1-1.tournament-gp-singapore-1", "Singapore" },
        { "aresriot.aws-rclusterprod-apse1-1.ap-gp-singapore-1", "Singapore 1" },
        { "aresriot.aws-rclusterprod-apse1-1.ap-gp-singapore-awsedge-1", "Singapore 2" },
        { "aresriot.aws-eun1-prod.eu-gp-stockholm-1", "Stockholm" },
        { "aresriot.aws-eun1-prod.tournament-gp-stockholm-1", "Stockholm" },
        { "aresriot.aws-rclusterprod-eun1-1.tournament-gp-stockholm-1", "Stockholm" },
        { "aresriot.aws-rclusterprod-eun1-1.eu-gp-stockholm-1", "Stockholm 1" },
        { "aresriot.aws-rclusterprod-eun1-1.eu-gp-stockholm-awsedge-1", "Stockholm 2" },
        { "aresqa.aws-apse2-dev.main1-gp-sydney-1", "Sydney" },
        { "aresqa.aws-apse2-dev.stage1-gp-sydney-1", "Sydney" },
        { "aresriot.aws-apse2-prod.ap-gp-sydney-1", "Sydney" },
        { "aresriot.aws-apse2-prod.ext1-gp-sydney-1", "Sydney" },
        { "aresriot.aws-apse2-prod.tournament-gp-sydney-1", "Sydney" },
        { "aresriot.aws-rclusterprod-apse2-1.ext1-gp-sydney-1", "Sydney" },
        { "aresriot.aws-rclusterprod-apse2-1.tournament-gp-sydney-1", "Sydney" },
        { "aresriot.aws-rclusterprod-apse2-1.ap-gp-sydney-1", "Sydney 1" },
        { "aresriot.aws-rclusterprod-apse2-1.ap-gp-sydney-awsedge-1", "Sydney 2" },
        { "arestencent.qcloud-tj1.alpha1-gp-1", "Tianjin 1" },
        { "arestencent.qcloud-tj1.alpha1-gp-2", "Tianjin 2" },
        { "aresriot.aws-apne1-prod.ap-gp-tokyo-1", "Tokyo" },
        { "aresriot.aws-apne1-prod.eu-gp-tokyo-1", "Tokyo" },
        { "aresriot.aws-apne1-prod.ext1-gp-kr1", "Tokyo" },
        { "aresriot.aws-apne1-prod.ext1-gp-tokyo-1", "Tokyo" },
        { "aresriot.aws-apne1-prod.tournament-gp-tokyo-1", "Tokyo" },
        { "aresriot.aws-rclusterprod-apne1-1.eu-gp-tokyo-1", "Tokyo" },
        { "aresriot.aws-rclusterprod-apne1-1.ext1-gp-kr1", "Tokyo" },
        { "aresriot.aws-rclusterprod-apne1-1.tournament-gp-tokyo-1", "Tokyo" },
        { "aresriot.aws-rclusterprod-apne1-1.ap-gp-tokyo-1", "Tokyo 1" },
        { "aresriot.aws-rclusterprod-apne1-1.ap-gp-tokyo-awsedge-1", "Tokyo 2" },
        { "aresqa.aws-usw2-dev.main1-gp-tournament-2", "Tournament" },
        { "aresriot.aws-atl1-prod.na-gp-atlanta-1", "US Central (Georgia 1)" },
        { "aresriot.aws-rclusterprod-atl1-1.na-gp-atlanta-1", "US Central (Georgia 2)" },
        { "aresriot.aws-rclusterprod-atl1-1.tournament-gp-atlanta-1", "US Central (Georgia)" },
        { "aresriot.aws-chi1-prod.na-gp-chicago-1", "US Central (Illinois)" },
        { "aresriot.aws-chi1-prod.tournament-gp-chicago-1", "US Central (Illinois)" },
        { "aresriot.aws-ord1-prod.na-gp-chicago-1", "US Central (Illinois)" },
        { "aresriot.aws-ord1-prod.tournament-gp-chicago-1", "US Central (Illinois)" },
        { "aresriot.mtl-riot-ord2-3.na-gp-chicago-1", "US Central (Illinois)" },
        { "aresriot.mtl-riot-ord2-3.tournament-gp-chicago-1", "US Central (Illinois)" },
        { "aresriot.aws-rclusterprod-dfw1-1.na-gp-dallas-1", "US Central (Texas)" },
        { "aresriot.aws-dfw1-prod.na-gp-dallas-1", "US Central (Texas)" },
        { "aresriot.aws-rclusterprod-dfw1-1.tournament-gp-dallas-1", "US Central (Texas)" },
        { "aresriot.aws-rclusterprod-use1-1.na-gp-ashburn-1", "US East (N. Virginia 1)" },
        { "aresriot.aws-rclusterprod-use1-1.na-gp-ashburn-awsedge-1", "US East (N. Virginia 2)" },
        { "aresriot.aws-rclusterprod-use1-1.ext1-gp-ashburn-1", "US East (N. Virginia)" },
        { "aresriot.aws-rclusterprod-use1-1.pbe-gp-ashburn-1", "US East (N. Virginia)" },
        { "aresriot.aws-rclusterprod-use1-1.tournament-gp-ashburn-1", "US East (N. Virginia)" },
        { "aresriot.aws-use1-prod.ext1-gp-ashburn-1", "US East (N. Virginia)" },
        { "aresriot.aws-use1-prod.na-gp-ashburn-1", "US East (N. Virginia)" },
        { "aresriot.aws-use1-prod.pbe-gp-ashburn-1", "US East (N. Virginia)" },
        { "aresriot.aws-use1-prod.tournament-gp-ashburn-1", "US East (N. Virginia)" },
        { "aresqa.aws-usw2-dev.sandbox1-gp-1", "US West" },
        { "globaltencent.tcc-sjc-dev.val-gp-1", "US West" },
        { "aresriot.aws-rclusterprod-usw1-1.na-gp-norcal-1", "US West (N. California 1)" },
        { "aresriot.aws-rclusterprod-usw1-1.na-gp-norcal-awsedge-1", "US West (N. California 2)" },
        { "aresriot.aws-rclusterprod-usw1-1.ext1-gp-na2", "US West (N. California)" },
        { "aresriot.aws-rclusterprod-usw1-1.pbe-gp-norcal-1", "US West (N. California)" },
        { "aresriot.aws-rclusterprod-usw1-1.tournament-gp-norcal-1", "US West (N. California)" },
        { "aresriot.aws-usw1-prod.ext1-gp-na2", "US West (N. California)" },
        { "aresriot.aws-usw1-prod.ext1-gp-norcal-1", "US West (N. California)" },
        { "aresriot.aws-usw1-prod.na-gp-norcal-1", "US West (N. California)" },
        { "aresriot.aws-usw1-prod.pbe-gp-norcal-1", "US West (N. California)" },
        { "aresriot.aws-usw1-prod.tournament-gp-norcal-1", "US West (N. California)" },
        { "aresriot.aws-rclusterprod-usw2-1.na-gp-oregon-1", "US West (Oregon 1)" },
        { "aresriot.aws-rclusterprod-usw2-1.na-gp-oregon-awsedge-1", "US West (Oregon 2)" },
        { "aresriot.aws-rclusterprod-usw2-1.pbe-gp-oregon-1", "US West (Oregon)" },
        { "aresriot.aws-rclusterprod-usw2-1.tournament-gp-oregon-1", "US West (Oregon)" },
        { "aresriot.aws-usw2-prod.na-gp-oregon-1", "US West (Oregon)" },
        { "aresriot.aws-usw2-prod.pbe-gp-oregon-1", "US West (Oregon)" },
        { "aresriot.aws-usw2-prod.tournament-gp-oregon-1", "US West (Oregon)" },
        { "aresqa.aws-usw2-dev.main1-gp-1", "US West 1" },
        { "aresqa.aws-usw2-dev.stage1-gp-1", "US West 1" },
        { "aresqa.aws-usw2-dev.main1-gp-4", "US West 2" },
        { "aresriot.aws-rclusterprod-waw1-1.eu-gp-warsaw-1", "Warsaw" },
        { "aresriot.aws-rclusterprod-waw1-1.tournament-gp-warsaw-1", "Warsaw" }
    };
     
     public static Dictionary<string, string> MapsById = new Dictionary<string, string>
        {
            {"7eaecc1b-4337-bbf6-6ab9-04b8f06b3319","Ascent"},
            {"d960549e-485c-e861-8d71-aa9d1aed12a2","Split"},
            {"b529448b-4d60-346e-e89e-00a4c527a405","Fracture"},
            {"2c9d57ec-4431-9c5e-2939-8f9ef6dd5cba","Bind"},
            {"2fb9a4fd-47b8-4e7d-a969-74b4046ebd53","Breeze"},
            {"fd267378-4d1d-484f-ff52-77821ed10dc2","Pearl"},
            {"e2ad5c54-4114-a870-9641-8ea21279579a","Icebox"},
            {"9c91a445-4f78-1baa-a3ea-8f8aadf4914d","Lotus"},
            {"ee613ee9-28b7-4beb-9666-08db13bb2244","The Range"},
            {"2bee0dc9-4ffe-519b-1cbd-7fbe763a6047","Haven"},
            {"690b3ed2-4dff-945b-8223-6da834e30d24","District"},
            {"12452a9d-48c3-0b02-e7eb-0381c3520404","Kasbah"},
            {"de28aa9b-4cbe-1003-320e-6cb3ec309557","Piazza"},
            {"92584fbe-486a-b1b2-9faa-39b0f486b498","Sunset"},
            {"2c09d728-42d5-30d8-43dc-96a05cc7ee9d","Drift"},
        };

        public static Dictionary<string, string> MapsByPath = new Dictionary<string, string>
        {
            {"/game/maps/ascent/ascent","Ascent"},
            {"/game/maps/bonsai/bonsai","Split"},
            {"/game/maps/canyon/canyon","Fracture"},
            {"/game/maps/duality/duality","Bind"},
            {"/game/maps/foxtrot/foxtrot","Breeze"},
            {"/game/maps/foxtrot_Copy/foxtrot_Copy","Breeze"},
            {"/game/maps/pitt/pitt","Pearl"},
            {"/game/maps/port/port","Icebox"},
            {"/game/maps/poveglia/range","The Range"},
            {"/game/maps/triad/triad","Haven"},
            {"/game/maps/jam/jam","Lotus"},
            {"/game/maps/hurm/hurm_alley/hurm_alley","District"},
            {"/game/maps/hurm/hurm_bowl/hurm_bowl","Kasbah"},
            {"/game/maps/hurm/hurm_yard/hurm_yard","Piazza"},
            {"/game/maps/juliett/juliett","Sunset"},
            {"/game/maps/hurm/hurm_helix/hurm_helix","Drift"},
            
        };
        
        public static Dictionary<string, string> AgentIdToNames = new Dictionary<string, string> 
        {
            {"eb93336a-449b-9c1b-0a54-a891f7921d69", "Phoenix"},
            {"6f2a04ca-43e0-be17-7f36-b3908627744d", "Skye"},
            {"117ed9e3-49f3-6512-3ccf-0cada7e3823b", "Cypher"},
            {"1e58de9c-4950-5125-93e9-a0aee9f98746", "Killjoy"},
            {"22697a3d-45bf-8dd7-4fec-84a9e28c69d7", "Chamber"},
            {"320b2a48-4d9b-a075-30f1-1f93a9b638fa", "Sova"},
            {"41fb69c1-4189-7b37-f117-bcaf1e96f1bf", "Astra"},
            {"569fdd95-4d10-43ab-ca70-79becc718b46", "Sage"},
            {"5f8d3a7f-467b-97f3-062c-13acf203c006", "Breach"},
            {"601dbbe7-43ce-be57-2a40-4abd24953621", "KAY/O"},
            {"707eab51-4836-f488-046a-cda6bf494859", "Viper"},
            {"7f94d92c-4234-0a36-9646-3a87eb8b5c89", "Yoru"},
            {"8e253930-4c05-31dd-1b6c-968525494517", "Omen"},
            {"8E253930-4C05-31DD-1B6C-968525494517", "Omen"},
            {"95b78ed7-4637-86d9-7e41-71ba8c293152", "Harbor"},
            {"9f0d8ba9-4140-b941-57d3-a7ad57c6b417", "Brimstone"},
            {"add6443a-41bd-e414-f6ad-e58d267f4e95", "Jett"},
            {"a3bfb853-43b2-7238-a4f1-ad90e9e46bcc", "Reyna"},
            {"f94c3b30-42be-e959-889c-5aa313dba261", "Raze"},
            {"dade69b4-4f5a-8528-247b-219e5a1facd6", "Fade"},
            {"bb2a4828-46eb-8cd1-e765-15848195d751", "Neon"},
            {"e370fa57-4757-3604-3648-499e1f642d3f", "Gekko"},
            {"cc8b64c8-4b25-4ff9-6e7f-37b4da43d235", "Deadlock"},
            {"0e38b510-41a8-5780-5e8f-568b2a4f2d6c", "Iso"},
        };
        
        public static Dictionary<int, string> RankNames = new Dictionary<int, string>
        {
            {0,"Unranked"},
            {1,"Unused 1"},
            {2,"Unused 2"},
            {3,"Iron 1"},
            {4,"Iron 2"},
            {5,"Iron 3"},
            {6,"Bronze 1"},
            {7,"Bronze 2"},
            {8,"Bronze 3"},
            {9,"Silver 1"},
            {10,"Silver 2"},
            {11,"Silver 3"},
            {12,"Gold 1"},
            {13,"Gold 2"},
            {14,"Gold 3"},
            {15,"Platinum 1"},
            {16,"Platinum 2"},
            {17,"Platinum 3"},
            {18,"Diamond 1"},
            {19,"Diamond 2"},
            {20,"Diamond 3"},
            {21,"Ascendant 1"},
            {22,"Ascendant 2"},
            {23,"Ascendant 3"},
            {24,"Immortal 1"},
            {25,"Immortal 2"},
            {26,"Immortal 3"},
            {27, "Radiant"}
        };
        
        public static async Task<PlayerPresence> GetPresenceData(ChatV4PresenceObj.Presence data)
        {
            if (data is null)
                return new PlayerPresence();
            
            
            if (string.IsNullOrEmpty(data.Private))
                return new PlayerPresence();
            byte[] stringData = Convert.FromBase64String(data.Private);
            string decodedString = Encoding.UTF8.GetString(stringData);
            return JsonSerializer.Deserialize<PlayerPresence>(decodedString);
        }

        public static string DetermineQueueKey(string codeNameQueue)
        {
            switch (codeNameQueue)
            {
                case "ggteam":
                    return "Escalation";
                case "deathmatch":
                    return "Deathmatch";
                case "spikerush":
                    return "Spike Rush";
                case "competitive":
                    return "Competitive";
                case "unrated":
                    return "Unrated";
                case "onefa":
                    return "Replication";
                case "swiftplay":
                    return "Swiftplay";
                case "snowball":
                    return "Snowball";
                case "lotus":
                    return "Lotus";
                case "newmap":
                    return "Sunset";
                case "premier-seasonmatch":
                    return "Premier";
                case "premier":
                    return "Premier";
                case "hurm":
                    return "Team Deathmatch";
                case "customgame":
                    return "Custom Game";
                default:
                    return "VALORANT";
            }
        }
}