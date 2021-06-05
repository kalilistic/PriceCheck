# PriceCheck [![Build](https://github.com/kalilistic/PriceCheck/actions/workflows/build.yml/badge.svg)](https://github.com/kalilistic/PriceCheck/actions/workflows/build.yml) [![Download count](https://img.shields.io/endpoint?url=https%3A%2F%2Fvz32sgcoal.execute-api.us-east-1.amazonaws.com%2FPriceCheck)](https://github.com/kalilistic/pricecheck) [![Crowdin](https://badges.crowdin.net/pricecheck/localized.svg)](https://crowdin.com/project/pricecheck)

Dalamud plugin to quickly check item prices. To check prices, hold your keybind and then hover over an item in your inventory or linked in chat. You can set your keybind (or disable it) in the PriceCheck settings. The prices are averages from Universalis and will not match any specific listings you see on the market board. You can use /pcheck to open the overlay and /pcheckconfig to open settings. If you need help, reach out on discord or open an issue on GitHub. If you want to help add translations, you can submit updates on Crowdin.

## Preview

![image](assets/preview.png)<br>

## Commands

**/pricecheck** or **/pcheck** to open overlay.<br>
**/pricecheckconfig** or **/pcheckconfig** to open settings.<br>

## Configuration

### General
**Plugin Enabled** - toggle the plugin off/on.<br>
**Show Prices** - show price or just show advice.<br>
**Show Unmarketable** - toggle whether to process items unmarketable items.<br>
**Hover Delay** - delay (in seconds) before processing after hovering.<br>
**Language** - use default or override plugin ui language.<br>

### Overlay
**Show Overlay** - show price check results in overlay window.<br>
**Use Overlay Colors** - use different colors for overlay based on result.<br>
**Max Items** - set max number of items in overlay at a time.<br>

### Chat
**Show in Chat** - show price check results in chat.<br>
**Use Chat Colors** - use different colors for chat based on result.<br>
**Use Item Links** - use item links in chat results.<br>

### Keybind
**Use Keybind** - toggle if keybind is used or just hover.<br>
**Modifier** - set your modifier key (e.g. shift).<br>
**Primary** - set your primary key (e.g. None, Z).<br>

### Thresholds
**Minimum Price** - set minimum price at which actual average will be displayed.<br>
**Max Upload Days** - set maximum age to avoid using old data.<br>
