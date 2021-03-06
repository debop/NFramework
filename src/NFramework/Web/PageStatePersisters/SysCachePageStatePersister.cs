﻿using System;
using System.Web.UI;
using NSoft.NFramework.Web.Caching;

namespace NSoft.NFramework.Web.PageStatePersisters {
    /// <summary>
    /// Web Page 상태를 서버 ASP.NET Cache에 압축 저장하도록 해주는 PageStatePersister 입니다.
    /// 
    /// 참고: http://www.eggheadcafe.com/tutorials/aspnet/b48875f6-525b-4267-a3a8-64dd33bfc2fe/keep-viewstate-out-of-pag.aspx
    /// 참고: http://www.codeproject.com/KB/viewstate/ViewStateTricks.aspx
    /// </summary>
    [Serializable]
    public class SysCachePageStatePersister : PageStatePersister<SysCacheRepository> //AbstractServerPageStatePersister
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        // private readonly SysCacheRepository _repository = new SysCacheRepository();

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="page"></param>
        public SysCachePageStatePersister(Page page) : base(page) {}

        ///// <summary>
        ///// ViewState 저장소로부터 저장된 ViewState 정보를 가져옵니다.
        ///// </summary>
        //protected override void LoadFromRepository()
        //{
        //	if(IsDebugEnabled)
        //		log.Debug("캐시에서 키에 해당하는 상태정보 로드를 시작합니다... StateValue=[{0}]", StateValue);

        //	if(StateValue.IsWhiteSpace())
        //		return;

        //	var cacheKey = GetCacheKey();
        //	var stateEntity = _repository.Get(cacheKey) as IPageStateEntity;

        //	if(stateEntity != null)
        //	{
        //		if(IsDebugEnabled)
        //			log.Debug("캐시에 저장된 정보를 로드했습니다...");

        //		object pageState;

        //		if(PageStateTool.TryParseStateEntity(stateEntity, Compressor, out pageState))
        //		{
        //			if(pageState != null && pageState is Pair)
        //			{
        //				ViewState = ((Pair) pageState).First;
        //				ControlState = ((Pair) pageState).Second;

        //				if(IsDebugEnabled)
        //					log.Debug("캐시에 저장된 상태정보를 로드하여, Page의 ViewState, ControlState에 설정했습니다.");
        //			}
        //		}
        //	}
        //	else
        //	{
        //		if(IsDebugEnabled)
        //			log.Debug("캐시에 지정된 키에 해당하는 상태정보가 저장되어 있지 않습니다. StateValue=[{0}]", StateValue);
        //	}
        //}

        ///// <summary>
        ///// Page의 ViewState 정보를 특정 저장소에 저장하고, 저장 토큰 값을 <see cref="AbstractServerPageStatePersister.StateValue"/>에 저장합니다.
        ///// </summary>
        //protected override void SaveToRepository()
        //{
        //	if(IsDebugEnabled)
        //		log.Debug("Page 상태정보를 Repository에 저장합니다...");

        //	if(StateValue.IsWhiteSpace())
        //		StateValue = CreateStateValue();

        //	var pageState = new Pair(ViewState, ControlState);
        //	var stateEntity = PageStateTool.CreateStateEntity(StateValue, pageState, Compressor, CompressThreshold);

        //	_repository.Set(GetCacheKey(), stateEntity, Expiration);

        //	if(IsDebugEnabled)
        //		log.Debug("Page 상태정보를 저장하였습니다!!! StateValue=[{0}]", StateValue);
        //}
    }
}