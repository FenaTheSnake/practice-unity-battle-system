using UnityEngine;
using Zenject;

public class MainSceneInstaller : MonoInstaller
{
    //[SerializeField] Map _map;

    public override void InstallBindings()
    {
        Container.Bind<Map>().AsSingle();
        Container.Bind<GameState>().AsSingle();
        Container.Bind<EnemyAI>().AsSingle().NonLazy();
    }
}
