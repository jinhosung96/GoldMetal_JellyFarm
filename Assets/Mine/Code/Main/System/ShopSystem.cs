using Mine.Code.App.Model;
using Mine.Code.Framework.Manager.Sound;
using Mine.Code.Jelly;
using Mine.Code.Main.Model;
using Mine.Code.Main.Setting;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mine.Code.Main.System
{
    public class ShopSystem : VObject<MainContext>
    {
        #region Fields

        [Inject] readonly JellyFarmJsonDBModel jellyFarmDBModel;
        [Inject] readonly CurrencyModel currencyModel;
        [Inject] readonly FieldModel fieldModel;
        [Inject] readonly UpgradeModel upgradeModel;
        [Inject] readonly MainFolderModel mainFolderModel;
        [Inject] readonly SaveSystem saveSystem;
        [Inject] readonly SoundManager soundManager;
        [Inject] readonly UISetting uISetting;
        [Inject] readonly MainSetting mainSetting;

        #endregion
    
        #region Properties

        public bool IsActiveSell { get; set; }

        #endregion
    
        #region Public Methods

        public void Sell(JellyContext jellyContext)
        {
            if (jellyContext != null)
            {
                soundManager.PlaySfx(uISetting.Sell);
            
                currencyModel.Gold.Value += jellyContext.Model.JellyPrice;
                jellyContext.Model.Despawn();
                Object.Destroy(jellyContext.gameObject);
            
                saveSystem.Save();
            }
        }

        public void Buy(int index)
        {
            if(fieldModel.Jellies.Count >= (upgradeModel.ApartmentLevel.Value + 1) * 2) return;
        
            var jellyCost = (int)jellyFarmDBModel.JellyPresets[index]["jellyCost"];
            if (currencyModel.Gold.Value >= jellyCost)
            {
                soundManager.PlaySfx(uISetting.Buy);
            
                currencyModel.Gold.Value -= jellyCost;
            
                var jellyPrefab = Resources.Load<GameObject>((string)jellyFarmDBModel.JellyPresets[index]["jellyPrefabPath"]);
                var jelly = Context.Container.Instantiate(jellyPrefab);
                var jellyModel = jelly.GetComponent<JellyContext>().Model;
                Context.Container.Inject(jellyModel);
                jelly.transform.SetParent(mainFolderModel.JellyFolder);
                jelly.transform.position = mainSetting.RandomPositionInField;
                jellyModel.Respawn();
            
                saveSystem.Save();
            }
            else soundManager.PlaySfx(uISetting.Fail);
        }
    
        public bool Unlock(int index)
        {
            var jellyPreset = jellyFarmDBModel.JellyPresets[index];
            var jellyCost = (int)jellyPreset["jellyCost"];
            if (currencyModel.Gelatin.Value >= jellyCost)
            {
                soundManager.PlaySfx(uISetting.UpgradeOrUnlock);
            
                currencyModel.Gelatin.Value -= jellyCost;
                jellyPreset["isUnlocked"] = true;

                saveSystem.Save();
                return true;
            }
            soundManager.PlaySfx(uISetting.Fail);
        
            return false;
        }

        #endregion
    }
}
