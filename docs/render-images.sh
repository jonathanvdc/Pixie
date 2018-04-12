#!/usr/bin/env bash

# To run this script, you'll need svg-term. More on that
# here: https://github.com/marionebl/svg-term-cli.

DIR=$(dirname $(realpath $0))
PARENT_DIR=$(dirname $DIR)

mkdir -p "$DIR/img"

svg-term --command "mono $PARENT_DIR/Examples/CaretDiagnostics/bin/clr/CaretDiagnostics.exe" \
    --at 2000 --height 10 --out "$DIR/img/caret.svg"

svg-term --command "mono $PARENT_DIR/Examples/PrintHelp/bin/clr/PrintHelp.exe" \
    --at 2000 --height 20 --out "$DIR/img/help-message.svg"

svg-term --command "mono $PARENT_DIR/Examples/FormattedList/bin/clr/FormattedList.exe fancy | head -n6 | tail -n5" \
    --at 2000 --height 5 --out "$DIR/img/degradation-fancy.svg"
svg-term --command "mono $PARENT_DIR/Examples/FormattedList/bin/clr/FormattedList.exe simple | head -n6 | tail -n5" \
    --at 2000 --height 5 --out "$DIR/img/degradation-simple.svg"

svg-term --command "mcs $PARENT_DIR/Pixie/Color.cs" \
    --at 2000 --height 3 --out "$DIR/img/sad-line-break.svg"

svg-term --command "mono $PARENT_DIR/Examples/SimpleErrorMessage/bin/clr/SimpleErrorMessage.exe" \
    --at 2000 --height 3 --out "$DIR/img/happy-line-break.svg"

svg-term --command "mono $PARENT_DIR/Examples/LoycInterop/bin/clr/LoycInterop.exe" \
    --at 2000 --height 6 --out "$DIR/img/loyc-interop.svg"
