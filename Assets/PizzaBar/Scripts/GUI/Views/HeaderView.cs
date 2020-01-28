using PizzaBar.Scripts.GUI.Models;
using PizzaBar.Scripts.Misc;
using ProjectCore.Misc;
using ProjectCore.ServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PizzaBar.Scripts.GUI.Views
{
    public class HeaderView : CachedBehaviour
    {
        [SerializeField] private Button _settingsButton;
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Slider _slider;

        private HeaderMenuModel _headerMenuModel;

        private void Awake()
        {
            _headerMenuModel = ServiceLocator.Resolve<HeaderMenuModel>();
            
            _settingsButton.onClick.AddListener(OnSettingButtonClicked);
            _headerMenuModel.Money.OnValueChanged += OnMoneyValueChanged;
            _headerMenuModel.Experience.OnValueChanged += OnExperienceValueChanged;
        }

        private void OnDestroy()
        {
            _settingsButton.onClick.RemoveAllListeners();
            _headerMenuModel.Money.OnValueChanged -= OnMoneyValueChanged;
            _headerMenuModel.Experience.OnValueChanged -= OnExperienceValueChanged;
        }

        private void OnSettingButtonClicked()
        {
            Debug.LogError("You looked advertising?");
        }

        private void OnMoneyValueChanged(int newMoneyAmount)
        {
            _moneyText.text = newMoneyAmount.ToString();
        }

        private void OnExperienceValueChanged(int newExperienceAmount)
        {
            var percentage = ExperienceConverter.GetPercentageForProgressBarAndCurrentLvl(newExperienceAmount, out var lvl);
            _slider.value = percentage;
            _levelText.text = "LEVEL " + lvl;
        }
    }
}