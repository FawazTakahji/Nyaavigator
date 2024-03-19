#!/bin/bash
read -p "Version: " version
pkg="Nyaavigator_${version}"
mkdir -p "${pkg}/DEBIAN"
mkdir -p "${pkg}/opt/nyaavigator"
mkdir -p "${pkg}/usr/share/applications"
mkdir -p "${pkg}/usr/share/icons"
cp nyaavigator.png "${pkg}/usr/share/icons/nyaavigator.png"

cat > "${pkg}/DEBIAN/control" <<EOF
Package: Nyaavigator
Version: ${version}
Essential: no
Priority: optional
Maintainer: Fawaz Takhji <fawaztakhji1233212004@gmail.com>
Architecture: amd64
Description: An app for browsing nyaa.si.
Depends: xdg-utils
EOF

cat > "${pkg}/DEBIAN/postinst" <<OUTER
#!/bin/bash

echo "Setting up files..."
sudo mkdir -p "/etc/nyaavigator"
if [ ! -f /etc/nyaavigator/Settings.json ]; then
    cat > "/etc/nyaavigator/Settings.json" <<EOF
{
  "SystemAccent": true,
  "CheckUpdates": true,
  "AccentColor": "#ff337ab7",
  "Theme": "System"
}
EOF
fi

sudo chmod -R a+rwx /etc/nyaavigator
exit 0
OUTER

chmod +x "${pkg}/DEBIAN/postinst"

cat > "${pkg}/DEBIAN/preinst" <<EOF
#!/bin/bash

echo "Looking for older version..."
if [ -f "/opt/nyaavigator" ]; then
    sudo rm /opt/nyaavigator/libHarfBuzzSharp.so
    sudo rm /opt/nyaavigator/libSkiaSharp.so
    sudo rm /opt/nyaavigator/Nyaavigator
    sudo rm /opt/nyaavigator/Nyaavigator.pdb
    sudo rmdir /opt/nyaavigator
fi
EOF

chmod +x "${pkg}/DEBIAN/preinst"

cat > "${pkg}/DEBIAN/postrm" <<'EOF'
#!/bin/bash

if [ "$1" != "remove" ]; then
  exit 0
fi

echo "Removing app files..."
sudo rm /etc/nyaavigator/Settings.json
sudo rmdir /etc/nyaavigator
EOF

chmod +x "${pkg}/DEBIAN/postrm"

cat > "${pkg}/usr/share/applications/nyaavigator.desktop" <<EOF
[Desktop Entry]
Version=1.0
Name=Nyaavigator
Comment=Browse nyaa.si
Exec=/opt/nyaavigator/Nyaavigator %u
Terminal=false
Type=Application
Categories=Utility
Icon=/usr/share/icons/nyaavigator.png
EOF

read -n1 -r -p "Copy your files then press any key to continue..."

dpkg-deb --build "${pkg}"