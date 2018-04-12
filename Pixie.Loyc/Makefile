files = \
	*.cs

libraries = \
	../Pixie/bin/clr/Pixie.dll \
	../packages/Loyc.Essentials.26.2.0/lib/net45/Loyc.Essentials.dll \
	../packages/Loyc.Collections.26.2.0/lib/net45/Loyc.Collections.dll \
	../packages/Loyc.Syntax.26.2.0/lib/net45/Loyc.Syntax.dll

ifeq ($(ASM_VERSION),)
	ASM_VERSION:=0.0.1.0
endif

common_options = -asm-name=Pixie.Loyc -asm-version=$(ASM_VERSION) -Wall -Wextra

.PHONY: all flo dll clean
all: flo dll

flo: bin/flo/Pixie.Loyc.flo
dll: bin/clr/Pixie.Loyc.dll

clean:
	rm -rf bin/
	rm -rf obj/

include ../flame-make-scripts/use-ecsc.mk

bin/flo/Pixie.Loyc.flo: $(files) $(libraries) | ecsc
	$(ECSC) $(files) -platform ir -runtime clr -libs $(libraries) -o $@ $(common_options)

bin/clr/Pixie.Loyc.dll bin/clr/Pixie.xml: $(files) $(libraries) | ecsc
	$(ECSC) $(files) -platform clr --docs -libs $(libraries) -o $@ $(common_options)
