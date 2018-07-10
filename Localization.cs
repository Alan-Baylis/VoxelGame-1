﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Language{English, Russian};
public enum GameMessage{GameSaved,GameLoaded, LoadingFailed}
public enum LocalizationKey{Level, Offline, PointsSec, Dig,StopDig,Gather,StopGather, RequiredSurface, Upgrade, UpgradeCost, Cancel}
public enum GameAnnouncements{NotEnoughResources};
public enum RestrictionKey{SideConstruction, UnacceptableSurfaceMaterial}
public enum RefusalReason {Unavailable, MaxLevel, HQ_RR1, HQ_RR2, HQ_RR3, HQ_RR4, HQ_RR5, HQ_RR6, SpaceAboveBlocked, NoBlockBelow }

public static class Localization {
	public static string rtype_nothing_name, rtype_nothing_descr, rtype_lumber_name, rtype_lumber_descr, rtype_stone_name, rtype_stone_descr,
	rtype_dirt_name, rtype_dirt_descr, rtype_food_name, rtype_food_descr, rtype_metalK_ore_name, rtype_metalK_descr, rtype_metalM_ore_name, rtype_metalM_descr,
	rtype_metalE_ore_name, rtype_metalE_descr, rtype_metalN_ore_name, rtype_metalN_descr, rtype_metalP_ore_name, rtype_metalP_descr,
	rtype_metalS_ore_name, rtype_metalS_descr, rtype_mineralF_descr, rtype_mineralL_descr, rtype_plastics_descr, rtype_concrete_name, rtype_concrete_descr,
	rtype_fertileSoil_name, rtype_fertileSoil_descr, rtype_fuel_name, rtype_fuel_descr, rtype_graphonium_name, rtype_graphonium_descr,
	rtype_supplies_name, rtype_supplies_descr;
	public static string ui_build, ui_dig_block, ui_pourIn, ui_clear, ui_storage_name, ui_stopWork, 
	ui_accept_destruction_on_clearing, ui_accept, ui_decline, ui_choose_block_action, ui_toPlain, ui_toGather, ui_cancelGathering, ui_workers, 
	ui_dig_in_progress, ui_clean_in_progress, ui_gather_in_progress, ui_pouring_in_progress, ui_activeSelf, ui_immigration, ui_trading, ui_buy, ui_sell,
	ui_selectResource, ui_immigrationEnabled, ui_immigrationDisabled, ui_immigrationPlaces, ui_close, ui_reset, ui_add_transaction, ui_setMode, ui_currentMode,
	ui_changeMaterial, ui_heightBlocked, ui_buildOnSideOnly, ui_freeSlots, ui_recruitmentInProgress, ui_showCrewCard, ui_showCrewsList, ui_noShip,
	ui_assemblyInProgress, ui_showVesselsList, ui_points_sec, ui_graphic_settings;
	public static string menu_colonyInfo, menu_gameMenuButton, menu_cancel, menu_save, menu_load;
	public static string info_housing, info_population, info_level, info_gearsCoefficient, info_hospitalsCoverage, info_happiness, info_health,
	info_birthrate;
	public static string announcement_powerFailure, announcement_starvation, announcement_peopleArrived, announcement_notEnoughResources,
	announcement_stillWind;
	public static string objects_left, extracted, work_has_stopped, sales_volume, min_value_to_trade, empty, vessel, change,cost;
	public static string[] structureName;
	public static string lowered_birthrate, normal_birthrate, improved_birthrate, material_required, no_activity, block,resources,coins;
	public static string rollingShop_gearsProduction, rollingShop_boatPartsProduction;
	public static string hangar_noShuttle, hangar_noCrew, hangar_hireCrew, hangar_hireCost, hangar_crewSalary, hangar_repairFor, hangar_readiness,
	hangar_refuel;
	public static string crew_membersCount,crew_stamina,crew_perception, crew_persistence, crew_bravery, crew_techSkills, crew_survivalSkills, crew_teamWork,
	crew_successfulMissions, crew_totalMissions;
	public static string quests_vesselsAvailable, quests_transmittersAvailable, quests_crewsAvailable, quests_vesselsRequired, quests_crewsRequired,
	quests_no_suitable_vessels;
	public static string mine_levelFinished;
	public static Language currentLanguage;

	static Localization() {
		structureName = new string[Structure.TOTAL_STRUCTURES_COUNT];
		ChangeLanguage(Language.English);
	}

	public static void ChangeLanguage(Language lan ) {
		switch (lan) {
		case Language.English:
			rtype_nothing_name = "Nothing";
			rtype_nothing_descr = "You shouldn't see this, it's a bug(";
			rtype_dirt_name = "Dirt";
			rtype_dirt_descr = "Organic cover of floating islands.";
			rtype_food_name = "Food";
			rtype_food_descr = "Organic fuel for your citizens.";
			rtype_lumber_name = "Wood";
			rtype_lumber_descr = "Different elastic wood, growing only in Last Sector Dominion. Used for building and sometimes decorating.";
			rtype_stone_name = "Stone";
			rtype_stone_descr = "Nature material used in construction. Processing into L-Concrete.";
			rtype_metalK_ore_name = "Metal K (ore)";
			rtype_metalK_descr = "Used in construction.";
			rtype_metalM_ore_name = "Metal M (ore)";
			rtype_metalM_descr = "Used in  machinery building.";
			rtype_metalE_ore_name = "Metal E (ore)";
			rtype_metalE_descr = "Used in electronic components production.";
			rtype_metalN_ore_name = "Metal N (ore)";
			rtype_metalN_descr = "Rare and expensive metal.";
			rtype_metalP_ore_name = "Metal P (ore)";
			rtype_metalP_descr = "Used in mass-production.";
			rtype_metalS_ore_name = "Metal S (ore)";
			rtype_metalS_descr = "Used in ship building.";
			rtype_mineralF_descr = "Used as fuel.";
			rtype_mineralL_descr = "Used to create plastic mass.";
			rtype_plastics_descr = "Easy-forming by special influence relatively tough material, used for building and manufacturing";
			rtype_concrete_name = "L-Concrete"; rtype_concrete_descr = "Comfortable and easy-forming building material.";
			rtype_fertileSoil_name = "Fertile Soil"; rtype_fertileSoil_descr = "Soil, appliable for growing edibles.";
			rtype_fuel_name = "Fuel"; rtype_fuel_descr = "Standart fuel for spaceship engine";
			rtype_graphonium_name = "Graphonium"; rtype_graphonium_descr = "Superstructured material, wrapping reality nearby";
			rtype_supplies_name = "Supplies"; rtype_supplies_descr = "Well-packed food, medicaments and another life-support goods.";

			structureName[Structure.PLANT_ID] = "Some plant"; 
			structureName[Structure.LANDED_ZEPPELIN_ID] = "Landed Zeppelin"; 
			structureName[Structure.STORAGE_0_ID] = "Primary storage";
			structureName[Structure.STORAGE_1_ID] = "Storage"; 
			structureName[Structure.STORAGE_2_ID] = "Storage"; 
			structureName[Structure.STORAGE_3_ID] = "Storage"; 
			structureName[Structure.STORAGE_5_ID] = "Storage"; 
			structureName[Structure.CONTAINER_ID] = "Container"; 
			structureName[Structure.MINE_ELEVATOR_ID] = "Mine elevator"; 
			structureName[Structure.LIFESTONE_ID] = "Life stone"; 
			structureName[Structure.HOUSE_0_ID] = "Tent"; 
			structureName[Structure.HOUSE_1_ID] = "Small house";
			structureName[Structure.HOUSE_2_ID] = "House";
			structureName[Structure.HOUSE_3_ID] = "Advanced house";
			structureName[Structure.HOUSE_5_ID] = "Residential Block";
			structureName[Structure.DOCK_ID] = "Basic dock"; 
			structureName[Structure.ENERGY_CAPACITOR_1_ID] = "Power capacitor"; 
			structureName[Structure.ENERGY_CAPACITOR_2_ID] = "Power capacitor"; 
			structureName[Structure.ENERGY_CAPACITOR_3_ID] = "Power capacitor"; 
			structureName[Structure.FARM_1_ID] = "Farm (lvl 1)"; 
			structureName[Structure.FARM_2_ID] = "Farm (lvl 2)"; 
			structureName[Structure.FARM_3_ID] = "Farm (lvl 3)"; 
			structureName[Structure.FARM_4_ID] = "Covered farm "; 
			structureName[Structure.FARM_5_ID] = "Farm Block "; 
			structureName[Structure.HQ_2_ID] = "HeadQuarters"; 
			structureName[Structure.HQ_3_ID] = "HeadQuarters"; 
			structureName[Structure.HQ_4_ID] = "HeadQuarters"; 
			structureName[Structure.LUMBERMILL_1_ID] = "Lumbermill"; 
			structureName[Structure.LUMBERMILL_2_ID] = "Lumbermill"; 
			structureName[Structure.LUMBERMILL_3_ID] = "Lumbermill"; 
			structureName[Structure.LUMBERMILL_4_ID] = "Covered lumbermill"; 
			structureName[Structure.LUMBERMILL_5_ID] = "Lumbermill Block"; 
			structureName[Structure.MINE_ID] = "Mine Entrance";
			structureName[Structure.SMELTERY_1_ID] = "Smeltery"; 
			structureName[Structure.SMELTERY_2_ID] = "Smeltery"; 
			structureName[Structure.SMELTERY_3_ID] = "Smelting Facility"; 
			structureName[Structure.SMELTERY_5_ID] = "Smeltery Block"; 
			structureName[Structure.WIND_GENERATOR_1_ID] = "Wind generator"; 
			structureName[Structure.BIOGENERATOR_2_ID] = "Biogenerator";
			structureName[Structure.HOSPITAL_2_ID] = "Hospital";
			structureName[Structure.MINERAL_POWERPLANT_2_ID] = "Mineral F powerplant";
			structureName[Structure.ORE_ENRICHER_2_ID] = "Ore enricher";
			structureName[Structure.ROLLING_SHOP_ID] = "Rolling shop";
			structureName[Structure.MINI_GRPH_REACTOR_ID] = "Small Graphonum reactor";
			structureName[Structure.FUEL_FACILITY_3_ID] = "Fuel facility";
			structureName[Structure.GRPH_REACTOR_4_ID] = "Graphonium reactor";
			structureName[Structure.PLASTICS_FACTORY_3_ID] = "Plastics factory";
			structureName[Structure.FOOD_FACTORY_4_ID] = "Food factory";
			structureName[Structure.FOOD_FACTORY_5_ID] = "Food factory Block";
			structureName[Structure.GRPH_ENRICHER_ID] = "Graphonium enricher";
			structureName[Structure.XSTATION_ID] = "Experimental station";
			structureName[Structure.QUANTUM_ENERGY_TRANSMITTER_ID] = "Quantum energy transmitter";
			structureName[Structure.CHEMICAL_FACTORY_ID] = "Chemical factory";
			structureName[Structure.RESOURCE_STICK_ID] = "Constructing block...";
			structureName[Structure.COLUMN_ID] = "Column";
			structureName[Structure.SWITCH_TOWER_ID] = "Switch tower";
			structureName[Structure.SHUTTLE_HANGAR_ID] = "Shuttle hangar";
			structureName[Structure.RECRUITING_CENTER_ID] = "Recruiting Center";
			structureName[Structure.EXPEDITION_CORPUS_ID] = "Expedition Corpus";
			structureName[Structure.QUANTUM_TRANSMITTER_ID] = "Quantum transmitter";

			ui_build = "Build"; ui_clear = "Clear"; ui_dig_block = "Dig block"; ui_pourIn = "Pour in";
			ui_storage_name = "Storage"; 
			ui_stopWork = "Stop works";
			ui_accept = "Yes"; ui_decline = "No";
			ui_choose_block_action = "Choose block action";
			ui_toPlain = "Plain ground"; ui_workers = "Workers : ";
			ui_toGather = "Gather resources"; ui_cancelGathering = "Stop gathering";
			ui_dig_in_progress = "Digging in progress"; ui_clean_in_progress = "Clearing in progress"; 
			ui_gather_in_progress = "Gathering in progress"; ui_pouring_in_progress = "Pouring in progress";
			ui_accept_destruction_on_clearing = "Demolish all buildings in zone?";
			ui_activeSelf = "Active";
			ui_immigration = "Immigration"; ui_trading = "Trading";
			ui_buy = "Buy"; ui_sell = "Sell"; 
			ui_selectResource = "Select resource"; ui_add_transaction = "Add transaction";
			ui_immigrationEnabled = "Immigration allowed"; ui_immigrationDisabled = "Immigration is not possible"; 
			ui_immigrationPlaces = "Immigrants count";
			ui_close = "Close"; ui_reset = "Reset";
			ui_setMode = "Set mode"; ui_currentMode = "Current mode";
			ui_changeMaterial = "Change material";
			ui_heightBlocked = "Height blocked";
			ui_buildOnSideOnly = "Can be placed only on side blocks";
			ui_freeSlots = "Free slots";
			ui_showCrewCard = "Show crew card";
			ui_showCrewsList = "Show crews list"; ui_showVesselsList = "Show vessels list";
			ui_noShip = "No ship";
			ui_assemblyInProgress = "Assembly in progress"; // shuttle assembling
			ui_points_sec = "points/sec";
			ui_graphic_settings = "Graphic settings";

			menu_colonyInfo = "Colony info"; menu_gameMenuButton = "Game menu"; menu_cancel = "Cancel";
			menu_save = "Save game"; menu_load = "Load game";
			info_housing = "Housing";
			info_population = "Population";
			info_level = " lvl.";
			info_gearsCoefficient = "Gears";
			info_happiness = "Happiness";
			info_hospitalsCoverage = "Hospitals coverage";
			info_health = "Health situation";
			info_birthrate = "Birthrate";

			announcement_powerFailure = "Power Failure";
			announcement_starvation = "People are starving!";
			announcement_peopleArrived = "New colonists arrived";
			announcement_notEnoughResources = "Not enough resources";
			announcement_stillWind = "No wind! All wind generators stopped";

			objects_left = "objects left";
			extracted = "extracted";
			work_has_stopped = "Work has stopped";
			sales_volume = "Sales volume";
			min_value_to_trade = "Limit";
			lowered_birthrate = "Low birthrate";
			normal_birthrate = "Normal birthrate";
			improved_birthrate = "Stimulate birthrate";
			material_required = "Required material: ";
			no_activity = "No activity";
			block = "block";
			vessel = "vessel";
			change = "change"; cost ="cost";

			mine_levelFinished = "Mine has finished new level";

			hangar_noShuttle = "No shuttle";
			hangar_noCrew = "No crew";
			hangar_hireCrew = "Hire crew";
			hangar_hireCost = "Hire cost";
			hangar_crewSalary = "Monthly payment";
			hangar_repairFor = "Repair for";
			resources = "resources";
			coins = "coins";
			hangar_readiness = "Readiness";
			hangar_refuel = "Refuel";

			crew_membersCount = "Members count";
			crew_stamina = "Stamina";
			crew_perception = "Perception";
			crew_persistence = "Persistence";
			crew_bravery = "Bravery";
			crew_techSkills = "Tech skills";
			crew_survivalSkills = "Survival skills";
			crew_teamWork = "Team work";
			crew_successfulMissions = "Successful missions";
			crew_totalMissions = "Total missions";

			quests_vesselsAvailable = "Vessels available";
			quests_transmittersAvailable = "Transmitters available";
			quests_crewsAvailable = "Crews available";
			quests_crewsRequired = "Crews required";
			quests_vesselsRequired = "Vessels required";
			quests_no_suitable_vessels = "No suitable vessels";

			rollingShop_gearsProduction = "Gears production";
			rollingShop_boatPartsProduction = "Boat parts production";

			currentLanguage = Language.English;
			break;
		case  Language.Russian: 
			ui_storage_name = "Склад";
			ui_accept_destruction_on_clearing = "Снести все здания в зоне покрытия?";
			ui_choose_block_action = "Выберите действие с блоком";
			ui_dig_block = "Выкопать блок";
			ui_pourIn = "Засыпать блок";
			ui_toPlain = "Разровнять"; ui_workers = "Рабочие : ";
			ui_toGather = "Собрать ресурсы"; ui_cancelGathering = "Остановить сбор ресурсов";
			ui_dig_in_progress = "Идет извлечение грунта"; ui_clean_in_progress = "Идет очистка"; 
			ui_gather_in_progress = "Идет сбор"; ui_pouring_in_progress = "Идет засыпка";
			ui_stopWork = "Остановить работы"; ui_activeSelf = "Работает";
			ui_immigration = "Иммиграция"; ui_trading = "Торговля";
			ui_buy = "Покупка"; ui_sell = "Продажа"; 
			ui_selectResource = "Выберите ресурс";  ui_add_transaction = "Добавить операцию";
			ui_immigrationEnabled = "Иммиграция разрешена"; ui_immigrationDisabled = "Въезд в город закрыт"; 
			ui_immigrationPlaces = "Мест для приёма иммигрантов";
			ui_close = "Закрыть"; ui_reset = "Сброс";
			ui_setMode = "Изменить режим"; ui_currentMode = "Текущий режим";

			menu_colonyInfo = "Состояние";
			info_housing = "Свободное жильё";
			info_population = "Население"; info_level = " ур.";
			info_gearsCoefficient = "Техническое оснащение";
			menu_gameMenuButton = "Меню"; menu_cancel = "Отмена";

			announcement_powerFailure = "Энергоснабжение нарушено!";
			announcement_starvation = "Закончилась провизия!";
			announcement_peopleArrived = "Прибыли новые поселенцы";
			announcement_notEnoughResources = "Недостаточно ресурсов!";
			announcement_stillWind = "Безветрие! Все ветрогенераторы остановились";

			objects_left = "осталось";
			extracted = "извлечено";
			work_has_stopped = "Работы остановлены";
			sales_volume = "Объём продажи";
			min_value_to_trade = "Ограничение";
			no_activity = "Бездействует"; // not a developer status!!!

			rollingShop_gearsProduction = "Производство оборудования";
			rollingShop_boatPartsProduction = "Производство комплектующих для шаттлов";

			currentLanguage = Language.Russian;
			break;	
		}
	}

	public static string GetGameMessage( GameMessage m) {
		string s = "";
		switch (m) {
		case GameMessage.GameSaved: 
			switch ( currentLanguage ) {
			case Language.English : s = "Game successfully saved"; break;
			case Language.Russian: s = "Игра сохранена"; break;
			}
			break;
		case GameMessage.GameLoaded:
			switch ( currentLanguage ) {
			case Language.English : s = "Game loaded"; break;
			case Language.Russian: s = "Игра загружена"; break;
			}
			break;
		case GameMessage.LoadingFailed:
			switch ( currentLanguage ) {
			case Language.English : s = "Loading failed"; break;
			case Language.Russian: s = "Не удалось загрузить"; break;
			}
			break;
		default:
			s = "...?";
			break;
		}
		return s;
	}

	public static string GetStructureName(int id ) {
		return structureName[id];
	}
	public static string GetStructureDescription(int id) {
		return "no descriptions yet";
	}
	public static string GetResourceName(int id) {
		switch (id) {
		default: return "Unregistered resource";
		case 0: return "Nothing"; 
		case ResourceType.DIRT_ID: return "Dirt"; 
		case ResourceType.FOOD_ID: return "Food"; 
		case ResourceType.LUMBER_ID : return "Wood";
		case ResourceType.STONE_ID : return "Stone";
		case ResourceType.METAL_K_ID : return "Metal K ";
		case ResourceType.METAL_M_ID : return "Metal K ";
		case ResourceType.METAL_E_ID : return "Metal K ";
		case ResourceType.METAL_N_ID : return "Metal K ";
		case ResourceType.METAL_P_ID : return "Metal K ";
		case ResourceType.METAL_S_ID : return "Metal K ";
		case ResourceType.METAL_K_ORE_ID : return "Metal K (ore)";
		case ResourceType.METAL_M_ORE_ID : return "Metal M (ore)";
		case ResourceType.METAL_E_ORE_ID : return "Metal E (ore)";
		case ResourceType.METAL_N_ORE_ID : return "Metal N (ore)";
		case ResourceType.METAL_P_ORE_ID : return "Metal P (ore)";
		case ResourceType.METAL_S_ORE_ID : return "Metal S (ore)";
		case ResourceType.MINERAL_F_ID : return "Mineral F";
		case ResourceType.MINERAL_L_ID : return "Mineral L";
		case ResourceType.PLASTICS_ID : return "Plastic";
		case ResourceType.CONCRETE_ID : return "L-Concrete";
		case ResourceType.FERTILE_SOIL_ID : return "Fertile soil";
		case ResourceType.FUEL_ID : return "Fuel";
		case ResourceType.GRAPHONIUM_ID : return "Graphonium";
		case ResourceType.SUPPLIES_ID : return "Supplies";			
		}
	}

	public static string GetAnnouncementString( GameAnnouncements announce) {
		switch (announce) {
		default: return "<announcement not found>";
		case GameAnnouncements.NotEnoughResources : return "Not enough resources!";
		}
	}

	public static string GetRestrictionPhrase(RestrictionKey rkey ) {
		switch (rkey) {
		default : return "Action not possible";
		case RestrictionKey.SideConstruction: return "Can be built only on side blocks";
		case RestrictionKey.UnacceptableSurfaceMaterial: return "Unacceptable surface material";
		}
	}


	public static string CostInCoins(float count) {
		switch (currentLanguage) {
		default:
		case Language.English: return count.ToString() + " coins";
		}
	}

	public static string AnnounceCrewReady( string name ) {
		switch (currentLanguage) {
		default:
		case Language.English: return "crew \" " + name + "\" ready";
		}
	}

	public static string NameCrew() { // waiting for креатив
		switch (currentLanguage) {
		default:
		case Language.English:		return "crew " + Crew.lastNumber.ToString();
		}
	}

	public static string NameShuttle() { // waiting for креатив
		switch (currentLanguage) {
		default:
		case Language.English:		return "shuttle "+ Shuttle.lastIndex.ToString();
		}
	}

	public static string GetWord(LocalizationKey key) {
		switch (key) {
		    case LocalizationKey.Level: return "level"; 
		    case LocalizationKey.Offline: return "offline";
		    case LocalizationKey.PointsSec: return "points/sec";
		    case LocalizationKey.Dig : return "Dig";
		    case LocalizationKey.StopDig: return "Stop digging";
		    case LocalizationKey.Gather: return "Gather";
		    case LocalizationKey.StopGather: return "Stop gathering";
		    case LocalizationKey.RequiredSurface : return "Required surface";
            case LocalizationKey.UpgradeCost: return "Upgrade cost";
            case LocalizationKey.Upgrade:return "Upgrade";
            case LocalizationKey.Cancel: return "Cancel";
		default: return "...";
		}
	}

    public static string GetRefusalReason(RefusalReason rr) {
        switch (rr) {
            default: return "bad developer guy prohibits it"; break;
            case RefusalReason.Unavailable: return "Unavailable";break;
            case RefusalReason.MaxLevel: return "Maximum level reached";break;
            case RefusalReason.HQ_RR1: return "No docks built";break;
            case RefusalReason.HQ_RR2: return "No rolling shops built";break;
            case RefusalReason.HQ_RR3: return "No graphonium enrichers built";break;
            case RefusalReason.HQ_RR4: return "No chemical factories";break;
            case RefusalReason.HQ_RR5: return "No reason, just prohibited;"; break;
            case RefusalReason.HQ_RR6: return "No reason, just prohibited;"; break;
            case RefusalReason.SpaceAboveBlocked: return "Space above blocked";break;
            case RefusalReason.NoBlockBelow: return "No block below";break;
        }
    }
}
