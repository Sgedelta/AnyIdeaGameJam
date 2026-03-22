extends Control

const PASSWORD = "1234"

@onready var label = $VBoxContainer/MarginContainer/Label
@onready var grid = $VBoxContainer/GridContainer

var input_text := ""

func _ready():
	for button in grid.get_children():
		if button is Button:
			button.pressed.connect(_on_button_pressed.bind(button.text))

func _on_button_pressed(value: String):
	if value == "Clear":
		input_text = ""
	elif value == "Enter":
		print("Correct" if input_text == PASSWORD else "Wrong")
		input_text = ""
	else:
		input_text += value

	label.text = input_text

func _input(event):
	if event is InputEventKey and event.pressed:
		var key_map = {
			KEY_KP_0: "0", KEY_KP_1: "1", KEY_KP_2: "2",
			KEY_KP_3: "3", KEY_KP_4: "4", KEY_KP_5: "5",
			KEY_KP_6: "6", KEY_KP_7: "7", KEY_KP_8: "8",
			KEY_KP_9: "9", KEY_KP_ENTER: "Enter", KEY_KP_PERIOD: "Clear"
		}

		if event.keycode in key_map:
			var value = key_map[event.keycode]

			for button in grid.get_children():
				if button is Button and button.text == value:
					button.button_pressed = true
					await get_tree().create_timer(0.1).timeout
					button.button_pressed = false

			_on_button_pressed(value)
