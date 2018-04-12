files = \
	*.cs

libraries = \
	../../Pixie/bin/clr/Pixie.dll \
	../../Pixie.Terminal/bin/clr/Pixie.Terminal.dll \
	../../Pixie.Loyc/bin/clr/Pixie.Loyc.dll \
	../../packages/Loyc.Essentials.26.2.0/lib/net45/Loyc.Essentials.dll \
	../../packages/Loyc.Collections.26.2.0/lib/net45/Loyc.Collections.dll \
	../../packages/Loyc.Syntax.26.2.0/lib/net45/Loyc.Syntax.dll \
	../../packages/Loyc.Ecs.26.2.0/lib/net45/Loyc.Ecs.dll

ifeq ($(ASM_VERSION),)
	ASM_VERSION:=0.0.1.0
endif

common_options = -asm-name=LoycInterop -asm-version=$(ASM_VERSION) -Wall -Wextra

.PHONY: all flo exe clean
all: flo exe

flo: bin/flo/LoycInterop.flo
exe: bin/clr/LoycInterop.exe

clean:
	rm -rf bin/
	rm -rf obj/

include ../../flame-make-scripts/use-ecsc.mk

bin/flo/LoycInterop.flo: $(files) $(libraries) | ecsc
	$(ECSC) $(files) -platform ir -runtime clr -libs $(libraries) -o $@ $(common_options)

bin/clr/LoycInterop.exe: $(files) $(libraries) | ecsc
	$(ECSC) $(files) -platform clr -libs $(libraries) -o $@ $(common_options)
