require "api_common"
function Init()
   c = container(scene_root, 0, 0, 100, 50, "CENTRE" )
   c.DrawBorder=false
   d = container(c, 0, 0, 200, 70, "CENTRE")
   d.DrawBorder = false
   d.ClipContents = true
   b = new_plain_button(d, "Launch", -200, 0, 100, 50, "CENTRE", "None", "btn_launch") 
   b.Alpha = 0;
   tween_pos(b, 0, 0, 1):Once():SetEase(ease.inSine);
   tween_alpha(b, 255, 2):Once():SetEase(ease.inSine);
   send_debug_message("HYPERLINK (UI debug) v0.5.18 \n -- Welcome!")
end
function OnButtonClick(sender, e)
    if sender == b then 
        load_new_scene(scene_manager, "game.scene")
    end
end

function OnUIUpdate()
    c.Width = scene_root.Width
    c.Height = scene_root.Height
end