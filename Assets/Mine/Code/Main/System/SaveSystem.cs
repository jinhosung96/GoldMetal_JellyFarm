using System;
using System.Collections.Generic;
using System.Linq;
using Mine.Code.App.Model;
using Mine.Code.Framework.Manager.Sound;
using Mine.Code.Jelly;
using Mine.Code.Main.Model;
using Mine.Code.Main.Setting;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mine.Code.Main.System
{
    public class SaveSystem : VObject<MainContext>, IStartable
    {
        #region Fields

        [Inject] readonly JellyFarmJsonDBModel jellyFarmDBModel;
        [Inject] readonly MainFolderModel mainFolderModel;
        [Inject] readonly FieldModel fieldModel;
        [Inject] readonly CurrencyModel currencyModel;
        [Inject] readonly UpgradeModel upgradeModel;
        [Inject] readonly SoundManager soundManager;
        [Inject] readonly MainSetting mainSetting;

        #endregion
    
        #region Entry Point

        void IStartable.Start()
        {
            fieldModel.Jellies = LoadJellies();
            Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ => Save());
        }

        #endregion

        #region Public Methods

        public void Save()
        {
            jellyFarmDBModel.DB["Currency"] = JObject.FromObject(currencyModel.Data);
            jellyFarmDBModel.DB["Field"]["jellies"] = JArray.FromObject(fieldModel.Jellies.Select(model => model.Data));
            jellyFarmDBModel.DB["Plant"] = JObject.FromObject(upgradeModel.Data);
            jellyFarmDBModel.SaveDB();
        }
    
        public List<JellyModel> LoadJellies()
        {
            return jellyFarmDBModel.Jellies.Select(data =>
            {
                var jellyPrefab = Resources.Load<GameObject>((string)jellyFarmDBModel.JellyPresets[(int)data["id"]]["jellyPrefabPath"]);
                var jelly = Context.Container.Instantiate(jellyPrefab);
                jelly.transform.SetParent(mainFolderModel.JellyFolder);
                jelly.transform.position = mainSetting.RandomPositionInField;
                var jellyContext = jelly.GetComponent<JellyContext>();
                var jellyModel = jellyContext.Model;
                Context.Container.Inject(jellyModel);
                jellyModel.Load(data.Value<int>("level"), data.Value<int>("exp"));
                return jellyModel;
            }).ToList();
        }
    
        #endregion
    }
}