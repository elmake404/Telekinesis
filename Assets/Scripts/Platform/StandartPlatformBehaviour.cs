using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandartPlatformBehaviour : PlatformBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform[] _enemySpawnPoints;
    [SerializeField] private Transform _interactObjectsPlaceholder;

    private SimpleObject[] _interactableObjects;

    public delegate void DelegateCreateNewZombie(ZombieBehaviour zombieBehaviour);
    public event DelegateCreateNewZombie OnCreateNewZombie;

    protected override void Start()
    {
        base.Start();
        InitInteractableObjects();
        InitEnemies();
    }

    private void InitInteractableObjects()
    {
        _interactableObjects = _interactObjectsPlaceholder.GetComponentsInChildren<SimpleObject>();
        for (int i = 0; i < _interactableObjects.Length; i++)
        {
            _interactableObjects[i].InitComponent();
        }
    }

    public void SubscribeDelegateCreateNewZombie(DelegateCreateNewZombie delegateCreateNewZombie)
    {
        OnCreateNewZombie += delegateCreateNewZombie;
    }

    private void InitEnemies()
    {
        for (int i = 0; i < _enemySpawnPoints.Length; i++)
        {
            GameObject instance = Instantiate(_enemyPrefab, _enemySpawnPoints[i].position, Quaternion.identity);
            instance.transform.SetParent(transform);
            instance.transform.position = _enemySpawnPoints[i].position;
            ZombieBehaviour zombieBehaviour = instance.GetComponent<ZombieBehaviour>();
            zombieBehaviour.SetSurvivorTransform(_survivalTransform);
            zombieBehaviour.SetPlatformTransform(transform);
            OnCreateNewZombie?.Invoke(zombieBehaviour);
        }
    }

}
