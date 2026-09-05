using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeapons : MonoBehaviour {
    public enum Weapon {
        axe,
        long_sword,
        sword,
        spear
    }

    [SerializeField] private GameObject axe;
    [SerializeField] private GameObject longSword;
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject spear;

    [SerializeField] private GameObject axe_image;
    [SerializeField] private GameObject longSword_image;
    [SerializeField] private GameObject sword_image;
    [SerializeField] private GameObject spear_image;
    [SerializeField] private GameObject button;

    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private Weapon secondWeapon;
    [SerializeField] private Weapon originalWeapon;

    private bool weapon_changed;
    private bool firstChangeBossPhase = true;
    public bool hasSpear;
    private bool bossPhaseActive;

    void Start() {
        currentWeapon = Weapon.sword;
        originalWeapon = Weapon.sword;
        hasSpear = false;
        HideAllWeaponImages();
    }

    void Update() {
        TrackCurrentWeapon();
        UpdateButtonVisibility();

        if (bossPhaseActive && Input.GetKeyDown(KeyCode.Alpha1)) {
            SwapWeapons();
        }
    }

    private void UpdateButtonVisibility() {
        bool shouldShowButton = bossPhaseActive && firstChangeBossPhase == false && HasActiveSecondWeapon();
        button.SetActive(shouldShowButton);
    }

    private bool HasActiveSecondWeapon() {
        switch (secondWeapon) {
            case Weapon.axe:
                return true;
            case Weapon.long_sword:
                return true;
            case Weapon.sword:
                return true;
            case Weapon.spear:
                return hasSpear; 
            default:
                return false;
        }
    }

    public void set_current_weapon(Weapon weapon) {
        currentWeapon = weapon;
        weapon_changed = true;

        if (weapon == Weapon.spear) {
            hasSpear = true;

            if (bossPhaseActive && secondWeapon == Weapon.spear) {
                secondWeapon = originalWeapon;
            }
        }

        if (bossPhaseActive) {
            UpdateWeaponUI();
        }
    }

    public Weapon get_current_weapon() { return currentWeapon; }

    public void OnSpearThrown() {
        hasSpear = false;

        if (currentWeapon == Weapon.spear) {
            Weapon temp = currentWeapon;
            currentWeapon = secondWeapon;
            secondWeapon = temp;
            weapon_changed = true;
        }
        else if (secondWeapon == Weapon.spear) {
            secondWeapon = currentWeapon;
        }

        if (bossPhaseActive) 
            UpdateWeaponUI();
    }

    private void HideAllWeaponImages() {
        axe_image.SetActive(false);
        longSword_image.SetActive(false);
        sword_image.SetActive(false);
        spear_image.SetActive(false);

        button.SetActive(false);
    }

    public void SwapWeapons() {
        if (bossPhaseActive) {
            if (secondWeapon == Weapon.spear && !hasSpear) {
                return;
            }

            Weapon temp = currentWeapon;
            set_current_weapon(secondWeapon);
            secondWeapon = temp;

            UpdateWeaponUI();
        }
    }

    public void CheckBossPhase(bool is_boss_phase) {
        bossPhaseActive = is_boss_phase;

        if (is_boss_phase && firstChangeBossPhase) {
            originalWeapon = currentWeapon;
            secondWeapon = currentWeapon;
            set_current_weapon(Weapon.spear);
            hasSpear = true;
            firstChangeBossPhase = false;
        }

        if (is_boss_phase) 
            UpdateWeaponUI();
        else if (!is_boss_phase) {
            HideAllWeaponImages();
            button.SetActive(false);
        }
    }

    private void UpdateWeaponUI() {
        HideAllWeaponImages();

        bool spearIsSecondWeapon = (secondWeapon == Weapon.spear && hasSpear);

        switch (secondWeapon) {
            case Weapon.axe:
                axe_image.SetActive(true);
                break;
            case Weapon.long_sword:
                longSword_image.SetActive(true);
                break;
            case Weapon.sword:
                sword_image.SetActive(true);
                break;
            case Weapon.spear:
                if (hasSpear) {
                    spear_image.SetActive(true);
                }
                break;
        }

        UpdateButtonVisibility();
    }

    private void TrackCurrentWeapon() {
        if (weapon_changed) {
            axe.SetActive(false);
            sword.SetActive(false);
            longSword.SetActive(false);
            spear.SetActive(false);

            switch (currentWeapon) {
                case Weapon.axe:
                    axe.SetActive(true);
                    break;
                case Weapon.long_sword:
                    longSword.SetActive(true);
                    break;
                case Weapon.sword:
                    sword.SetActive(true);
                    break;
                case Weapon.spear:
                    if (hasSpear)
                        spear.SetActive(true);
                    break;
            }
            weapon_changed = false;
        }
    }
}

