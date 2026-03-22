extends Control

const PASSWORD = "↑↑↑↑"

@onready var label = $VBoxContainer/MarginContainer/Label
@onready var grid = $VBoxContainer/GridContainer

var input_text := ""

func _ready():
	for button in grid.get_children():
		if button is Button:
			button.pressed.connect(_on_button_pressed.bind(button.text))

func _on_button_pressed(value: String):
	input_text += value
	label.text = input_text
	await get_tree().create_timer(0.1).timeout
	_check_input()
	
func _check_input():
	if input_text == PASSWORD:
		print("YAY")
		input_text = ""
		label.text = input_text
		
	if !PASSWORD.begins_with(input_text):
		print("WRONG")
		input_text = ""
		label.text = input_text

func _input(event):
	var value: String = ""
	if event is InputEventJoypadButton and event.pressed:
		var joy_map = {
			11: "↑", 13: "←", 12: "↓", 14: "→"
		}

		if event.button_index in joy_map:
			value = joy_map[event.button_index]

			for button in grid.get_children():
				if button is Button and button.text == value:
					button.button_pressed = true
					await get_tree().create_timer(0.1).timeout
					button.button_pressed = false

			_on_button_pressed(value)
