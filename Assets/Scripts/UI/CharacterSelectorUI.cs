using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[DisallowMultipleComponent]
public class CharacterSelectorUI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Populate with the child CharacterSelector gameobject")]
    #endregion
    [SerializeField] private Transform characterSelector;
    #region Tooltip
    [Tooltip("Populate with the TextMeshPro component on the PlayerNameInput game object")]
    #endregion
    [SerializeField] private TMP_InputField playerNameInput;
    private List<PlayerDetailsSO> playerDetailsSOList;
    private GameObject playerSelectionPrefab;
    private CurrentPlayerSO currentPlayerSO;
    private List<GameObject> playerCharacterGameObjectList = new List<GameObject>();
    private Coroutine coroutine;
    private int selectedPlayerIndex = 0;
    private float offset = 4f;

    private void Awake()
    {
        playerSelectionPrefab = GameResources.Instance.playerSelectionPrefab;
        playerDetailsSOList = GameResources.Instance.playerDetailsList;
        currentPlayerSO = GameResources.Instance.currentPlayer;
    }

    private void Start()
    {
        for (int i = 0; i < playerDetailsSOList.Count; i++)
        {
            GameObject playerSelectionObject = Instantiate(playerSelectionPrefab, characterSelector);
            playerCharacterGameObjectList.Add(playerSelectionObject);
            playerSelectionObject.transform.localPosition = new Vector3(offset * i, 0f, 0f);
            PopulatePlayerDetails(playerSelectionObject.GetComponent<PlayerSelectionUI>(), playerDetailsSOList[i]);
        }

        playerNameInput.text = currentPlayerSO.name;

        currentPlayerSO.playerDetails = playerDetailsSOList[selectedPlayerIndex];
    }

    private void PopulatePlayerDetails(PlayerSelectionUI playerSelectionUI, PlayerDetailsSO playerDetailsSO)
    {
        playerSelectionUI.playerHandSpriteRenderer.sprite = playerDetailsSO.playerHandSprite;
        playerSelectionUI.playerHandNoWeaponSpriteRenderer.sprite = playerDetailsSO.playerHandSprite;
        playerSelectionUI.playerWeaponSpriteRenderer.sprite = playerDetailsSO.startingWeapon.weaponSprite;
        playerSelectionUI.animator.runtimeAnimatorController = playerDetailsSO.runtimeAnimatorController;
    }

    public void NextCharacter()
    {
        if (selectedPlayerIndex >= playerDetailsSOList.Count - 1) return;

        selectedPlayerIndex++;
        currentPlayerSO.playerDetails = playerDetailsSOList[selectedPlayerIndex];

        MoveToSelectedCharacter(selectedPlayerIndex);
    }

    public void PreviousCharacter()
    {
        if (selectedPlayerIndex == 0) return;

        selectedPlayerIndex--;
        currentPlayerSO.playerDetails = playerDetailsSOList[selectedPlayerIndex];

        MoveToSelectedCharacter(selectedPlayerIndex);
    }

    private void MoveToSelectedCharacter(int index)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(MoveToSelectedCharacterRoutine(index));
    }

    private IEnumerator MoveToSelectedCharacterRoutine(int index)
    {
        float currentLocalXPosition = characterSelector.localPosition.x;
        float targetLocalXPosition = index * offset * characterSelector.localScale.x * (-1f);

        while (Math.Abs(currentLocalXPosition - targetLocalXPosition) > 0.01f) {
            currentLocalXPosition = Mathf.Lerp(currentLocalXPosition, targetLocalXPosition, Time.deltaTime * 10f);

            characterSelector.localPosition = new Vector3(currentLocalXPosition, characterSelector.localPosition.y, 0f);
            yield return null;
        }

        characterSelector.localPosition = new Vector3(targetLocalXPosition, characterSelector.localPosition.y, 0f);
    }

    public void UpdatePlayerName()
    {
        playerNameInput.text = playerNameInput.text.ToUpper();

        currentPlayerSO.playerName = playerNameInput.text;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(characterSelector), characterSelector);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerNameInput), playerNameInput);
    }
#endif
    #endregion
}
