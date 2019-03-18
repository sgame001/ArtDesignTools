using CatLib;
using HKLibrary;

public class SMCServiceProvider : IServiceProvider {

	public void Init()
	{
		
	}

	public void Register()
	{
		App.Singleton<HKComboSkinMesh>().Alias<ISkinMeshCombo>();
	}
}
