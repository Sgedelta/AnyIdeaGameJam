extends Control

var PASSWORD = "1234"
signal password_correct

@onready var label = $VBoxContainer/MarginContainer/Label
@onready var grid = $VBoxContainer/GridContainer

var input_text := ""
var showing := false

@export var show_pos := Vector2(0,0)
@export var hide_pos := Vector2(0, 400)

func set_password(new_pass: String):
	PASSWORD = new_pass
	reset()

func _ready():
	for button in grid.get_children():
		if button is Button:
			button.pressed.connect(_on_button_pressed.bind(button.text))
		hide_ui()

func _on_button_pressed(value: String):
	if value == "Clear":
		input_text = ""
	elif value == "Enter":
		if input_text == PASSWORD:
			input_text = "CORRECT"
			label.text = input_text
			await get_tree().create_timer(0.2).timeout
			password_correct.emit()
			reset()
		else:
			input_text = "WRONG"
			label.text = input_text
			await get_tree().create_timer(0.2).timeout
			reset()
	else:
		input_text += value

	label.text = input_text

func reset():
	input_text = ""
	label.text = input_text

func _input(event):
	if !showing:
		return

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
