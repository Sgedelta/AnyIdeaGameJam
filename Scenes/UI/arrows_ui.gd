extends Control

const PASSWORD = "↑↑↑↑"

@onready var label = $VBoxContainer/MarginContainer/Label
@onready var grid = $VBoxContainer/GridContainer

var input_text := ""
var showing := false

@export var show_pos := Vector2(0,0)
@export var hide_pos := Vector2(0, 400)

func _ready():
	for button in grid.get_children():
		if button is Button:
			button.pressed.connect(_on_button_pressed.bind(button.text))
	hide_ui()

func _on_button_pressed(value: String):
	input_text += value
	label.text = input_text
	await get_tree().create_timer(0.1).timeout
	_check_input()
	
func _check_input():
	if input_text == PASSWORD:
		print("YAY")
		reset()
		
	if !PASSWORD.begins_with(input_text):
		print("WRONG")
		reset()

func reset():
	input_text = ""
	label.text = input_text

func _input(event):
	if !showing:
		return
	
	if event is InputEventJoypadButton and event.pressed:
		var joy_map = {
			11: "↑", 13: "←", 12: "↓", 14: "→"
		}

		if event.button_index in joy_map:
			var value = joy_map[event.button_index]

			for button in grid.get_children():
				if button is Button and button.text == value:
					button.button_pressed = true
					await get_tree().create_timer(0.1).timeout
					button.button_pressed = false

			_on_button_pressed(value)
			return
	
func show_ui():
	if showing:
		return
	showing = true
	reset()
	visible = true
	position = show_pos
	
func hide_ui():
	if !showing:
		return
	showing = false
	reset()
	visible = false
	position = hide_pos
